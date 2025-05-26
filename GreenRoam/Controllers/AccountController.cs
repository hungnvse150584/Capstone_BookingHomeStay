using BusinessObject.IdentityModel;
using BusinessObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using Service.IService;
using System.Text;
using Service.Service;
using Microsoft.AspNetCore.Authorization;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Response.Accounts;

namespace GreenRoam.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Account> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<Account> _signinManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStaffService _staffService;
        private readonly IAccountService _accountService;

        public AccountController(UserManager<Account> userManager, ITokenService tokenService,
            SignInManager<Account> signInManager, RoleManager<IdentityRole> roleManager, IStaffService staffService, 
            IAccountService accountService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
            _roleManager = roleManager;
            _staffService = staffService;
            _accountService = accountService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetTotalAccount")]
        public async Task<BaseResponse<GetTotalAccount>> GetTotalAccount()
        {
            return await _accountService.GetTotalAccount();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username!");

            var userEmail = await GetUser(user.Email);
            bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(userEmail);
            if (!isEmailConfirmed) return BadRequest("You need to confirm email before login");

            if (user.Status == false)
            {
                return Unauthorized("Cannot login with this account anymore!");
            }


            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any())
            {
                return BadRequest("Sorry, Your account has Proplem to Identify who you are");
            }

            int? homeStayId = null;
            if (roles.Contains("Staff"))
            {
                var staffResult = await _staffService.GetStaffByID(user.Id);

                if (staffResult == null || staffResult.Data == null)
                {
                    return BadRequest("Staff account does not have a HomeStay assigned.");
                }

                homeStayId = staffResult.Data.HomeStayID;
            }
            var token = await _tokenService.createToken(user);
            return Ok(
                new NewUserDto
                {
                    UserID = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Phone = user.Phone,
                    Name = user.Name,
                    Address = user.Address,
                    HomeStayID = homeStayId,
                    Token = token.AccessToken,
                    RefreshToken = token.RefreshToken
                }
            );
        }


        [HttpPost("register-Customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var accountApp = new Account
                {
                    UserName = registerDto.Username,
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Address = registerDto.Address,
                    Phone = registerDto.Phone,
                    BankAccountNumber = registerDto.BankAccountNumber,
                    Status = true
                };

                var existUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existUser != null)
                {
                    return BadRequest("This email has already registered, please try another email!!!");
                }
                else
                {
                    var createdUser = await _userManager.CreateAsync(accountApp, registerDto.Password);

                    if (createdUser.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(accountApp, "CUSTOMER");
                        var token = await _tokenService.createToken(accountApp);
                        if (roleResult.Succeeded)
                        {
                            var _user = await GetUser(registerDto.Email);
                            var emailCode = await _userManager.GenerateEmailConfirmationTokenAsync(_user!);

                            if (string.IsNullOrEmpty(emailCode))
                            {
                                await _userManager.DeleteAsync(accountApp); // Xóa tài khoản để tránh bị kẹt
                                return StatusCode(500, "Failed to generate confirmation token. Please try again.");
                            }

                            string sendEmail = SendEmail(_user!.Email!, emailCode);

                            if (string.IsNullOrEmpty(sendEmail)) // Nếu gửi thất bại
                            {
                                await _userManager.DeleteAsync(accountApp); // Xóa tài khoản để tránh bị kẹt
                                return StatusCode(500, "Failed to send email. Please try again.");
                            }

                            var userRoles = await _userManager.GetRolesAsync(accountApp);
                            return Ok(
                                new NewUserDto
                                {
                                    UserName = accountApp.UserName,
                                    Email = accountApp.Email,
                                    Name = accountApp.Name,
                                    Address = accountApp.Address,
                                    Phone = accountApp.Phone,
                                    Roles = userRoles.ToList(),
                                    Token = token.AccessToken,
                                    RefreshToken = token.RefreshToken
                                }
                            );
                        }
                        else
                        {
                            return StatusCode(500, roleResult.Errors);
                        }

                    }
                    else
                    {
                        return StatusCode(500, createdUser.Errors);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("register-Owner")]
        public async Task<IActionResult> RegisterOwner([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var accountApp = new Account
                {
                    UserName = registerDto.Username,
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Address = registerDto.Address,
                    Phone = registerDto.Phone,
                    TaxCode = registerDto.TaxCode,
                    BankAccountNumber = registerDto.BankAccountNumber,
                    Status = true
                };

                var existUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existUser != null)
                {
                    return BadRequest("This email has already registered, please try another email!!!");
                }
                else
                {
                    var createdUser = await _userManager.CreateAsync(accountApp, registerDto.Password);

                    if (createdUser.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(accountApp, "OWNER");
                        var token = await _tokenService.createToken(accountApp);
                        if (roleResult.Succeeded)
                        {
                            var _user = await GetUser(registerDto.Email);
                            var emailCode = await _userManager.GenerateEmailConfirmationTokenAsync(_user!);

                            if (string.IsNullOrEmpty(emailCode))
                            {
                                await _userManager.DeleteAsync(accountApp); // Xóa tài khoản để tránh bị kẹt
                                return StatusCode(500, "Failed to generate confirmation token. Please try again.");
                            }

                            string sendEmail = SendEmail(_user!.Email!, emailCode);

                            if (string.IsNullOrEmpty(sendEmail)) // Nếu gửi thất bại
                            {
                                await _userManager.DeleteAsync(accountApp); // Xóa tài khoản để tránh bị kẹt
                                return StatusCode(500, "Failed to send email. Please try again.");
                            }

                            var userRoles = await _userManager.GetRolesAsync(accountApp);
                            return Ok(
                                new NewUserDto
                                {
                                    UserName = accountApp.UserName,
                                    Email = accountApp.Email,
                                    Name = accountApp.Name,
                                    Address = accountApp.Address,
                                    Phone = accountApp.Phone,
                                    Taxcode = accountApp.TaxCode,
                                    BankAccountNumber = accountApp.BankAccountNumber,
                                    Roles = userRoles.ToList(),
                                    Token = token.AccessToken,
                                    RefreshToken = token.RefreshToken
                                }
                            );
                        }
                        else
                        {
                            return StatusCode(500, roleResult.Errors);
                        }

                    }
                    else
                    {
                        return StatusCode(500, createdUser.Errors);
                    }
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
        private string SendEmail(string email, string emailCode)
        {
            StringBuilder emailMessage = new StringBuilder();
            emailMessage.Append("<html>");
            emailMessage.Append("<body>");
            emailMessage.Append($"<p>Dear {email},</p>");
            emailMessage.Append("<p>Thank you for your registering with us. To verifiy your email address, please use the following verification code: </p>");
            emailMessage.Append($"<h2>Verification Code: {emailCode}</h2>");
            emailMessage.Append("<p>Please enter this code on our website to complete your registration.</p>");
            emailMessage.Append("<p>If you did not request this, please ignore this email</p>");
            emailMessage.Append("<br>");
            emailMessage.Append("<p>Best regards,</p>");
            emailMessage.Append("<p><strong>ChoTot-Travel-CTT</strong></o>");
            emailMessage.Append("</body>");
            emailMessage.Append("</html>");

            string message = emailMessage.ToString();
            var _email = new MimeMessage();
            _email.To.Add(MailboxAddress.Parse(email));
            _email.From.Add(MailboxAddress.Parse("khanhvmse171632@fpt.edu.vn"));
            _email.Subject = "Email Confirmation";
            _email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("khanhvmse171632@fpt.edu.vn", "qpvj xjhk eihq sptw"); //user email and password qpvj xjhk eihq sptw
            smtp.Send(_email);
            smtp.Disconnect(true);
            return "Thank you for your registration, kindly check your email for confirmation code";

        }

        [HttpPost("confirmation/{email}/{code:int}")]
        public async Task<IActionResult> Confirmation(string email, int code)
        {
            if (string.IsNullOrEmpty(email) || code <= 0)
            {
                return BadRequest("Invalid Code Provided");
            }
            var user = await GetUser(email);
            if (user == null)
            {
                return BadRequest("Invalid Indentity Provided");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code.ToString());
            if (!result.Succeeded)
            {
                return BadRequest("Invalid Code Provided");
            }
            else
            {
                return Ok("Email confirm successfully, you can proceed to login");
            }
        }

        private async Task<Account> GetUser(string email)
        => await _userManager.FindByEmailAsync(email);

        [HttpPost("resetToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var result = await _tokenService.renewToken(model);
            return Ok(new ApiResponse
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Data
            });

        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("create account")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (string.IsNullOrEmpty(createAccountDto.Role))
                    return BadRequest("Role is required.");

                var existUser = await _userManager.FindByEmailAsync(createAccountDto.Email);
                if (existUser != null)
                {
                    return BadRequest("This email has already registered, please try another email!!!");
                }

                var accountApp = new Account
                {
                    UserName = createAccountDto.Username,
                    Name = createAccountDto.Name,
                    Email = createAccountDto.Email,
                    Address = createAccountDto.Address,
                    Phone = createAccountDto.Phone,
                    BankAccountNumber = createAccountDto.BankAccountNumber,
                    TaxCode = createAccountDto.TaxCode,
                    Status = true,
                    EmailConfirmed = true
                };

                var role = createAccountDto.Role.ToUpper();

                if (role == "ADMIN")
                {
                    return BadRequest("Cannot create account with the role 'Admin'.");
                }

                var createdUser = await _userManager.CreateAsync(accountApp, createAccountDto.Password);

                if (createdUser.Succeeded)
                {

                    var roleExists = await _roleManager.RoleExistsAsync(role);
                    if (roleExists)
                    {
                        if (createAccountDto.Role.ToUpper() == "ADMIN")
                        {
                            return BadRequest("Cannot create account with role 'Admin'.");
                        }
                        var roleResult = await _userManager.AddToRoleAsync(accountApp, createAccountDto.Role);

                        if (roleResult.Succeeded)
                        {
                            var userRoles = await _userManager.GetRolesAsync(accountApp);
                            var token = await _tokenService.createToken(accountApp);
                            return Ok(
                                new NewUserDto
                                {
                                    UserID = accountApp.Id,
                                    UserName = accountApp.UserName,
                                    Email = accountApp.Email,
                                    Name = accountApp.Name,
                                    Address = accountApp.Address,
                                    Phone = accountApp.Phone,
                                    BankAccountNumber = accountApp.BankAccountNumber,
                                    Taxcode = accountApp.TaxCode,
                                    Roles = userRoles.ToList(),
                                    Token = token.AccessToken,
                                    RefreshToken = token.RefreshToken
                                }
                            );
                        }
                        else
                        {
                            return StatusCode(500, roleResult.Errors);
                        }
                    }
                    else
                    {
                        await _userManager.DeleteAsync(accountApp);
                        return BadRequest($"Role '{createAccountDto.Role}' does not exist.");
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Account")]
        public async Task<IActionResult> UpdateAccount(string userId, [FromBody] UpdateAccountDto updateAccountDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound($"User with ID '{userId}' not found.");
                }
                if (!string.IsNullOrEmpty(updateAccountDto.Username))
                {
                    user.UserName = updateAccountDto.Username;
                }

                // Update role if provided
                if (!string.IsNullOrEmpty(updateAccountDto.Role))
                {
                    // Check if the new role exists
                    var newRole = updateAccountDto.Role.ToUpper();
                    var roleExists = await _roleManager.RoleExistsAsync(newRole);
                    if (!roleExists)
                    {
                        return BadRequest($"Role '{newRole}' does not exist.");
                    }

                    // Remove current roles and assign new role
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var roleRemoveResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!roleRemoveResult.Succeeded)
                    {
                        return StatusCode(500, roleRemoveResult.Errors);
                    }

                    var roleAddResult = await _userManager.AddToRoleAsync(user, newRole);
                    if (!roleAddResult.Succeeded)
                    {
                        return StatusCode(500, roleAddResult.Errors);
                    }
                }

                // Update other account properties
                user.Name = updateAccountDto.Name ?? user.Name;
                user.Address = updateAccountDto.Address ?? user.Address;
                user.Phone = updateAccountDto.Phone ?? user.Phone;
                user.BankAccountNumber = updateAccountDto.BankAccountNumber ?? user.BankAccountNumber;

                // Save changes
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    // Optionally, return updated user information
                    var updatedUserRoles = await _userManager.GetRolesAsync(user);
                    return Ok(new NewUserDto
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Name = user.Name,
                        Address = user.Address,
                        Phone = user.Phone,
                        BankAccountNumber = user.BankAccountNumber,
                        Roles = updatedUserRoles.ToList()
                    });
                }
                else
                {
                    return StatusCode(500, updateResult.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update-Account-Status")]
        public async Task<IActionResult> UpdateAccount(string userEmail, [FromBody] UpdateAccountStatusDto updateAccountStatusDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound($"User with Email '{userEmail}' not found.");
                }
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("ADMIN"))
                {
                    return BadRequest("Admin cannot banned by him/herself");
                }
                user.Status = updateAccountStatusDto.Status;
                var updateResult = await _userManager.UpdateAsync(user);
                if (updateResult.Succeeded)
                {
                    return Ok(new UpdateAccountStatusResponse
                    {
                        Email = user.Email,
                        Status = user.Status
                    });
                }
                else
                {
                    return StatusCode(500, updateResult.Errors);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get-all-accounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                // Retrieve all user accounts asynchronously
                var allAccounts = await _userManager.Users.ToListAsync();

                // Map to DTOs for response
                var accountInfoList = allAccounts.Select(user => new NewUserDto
                {
                    UserID = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Address = user.Address,
                    BankAccountNumber = user.BankAccountNumber,
                    Taxcode = user.TaxCode,
                    Phone = user.Phone,
                    isActive = user.Status,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList() // This should ideally be await _userManager.GetRolesAsync(user)
                }).ToList();

                return Ok(accountInfoList);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Failed to retrieve account information: {e.Message}");
            }
        }

        [Authorize(Roles = "Customer, Owner, Staff")]
        [HttpPost("Reset-Password-Token")]
        public async Task<IActionResult> ResetPasswordToken([FromBody] ResetTokenModel resetTokenModel)
        {
            var user = await _userManager.FindByEmailAsync(resetTokenModel.Email);
            if (user == null)
            {
                return BadRequest("Cannot find Email. Please check again!");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new { token = token });
        }

        [Authorize(Roles = "Customer, Owner, Staff")]
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetToken resetToken)
        {
            var user = await _userManager.FindByEmailAsync(resetToken.Email);
            if (user == null)
            {
                return BadRequest("UserName is wrong, Please check again!");
            }

            user = await _userManager.FindByNameAsync(resetToken.Username);
            if (user == null)
            {
                return BadRequest("Cannot find Email, Please check again!");
            }

            if (string.Compare(resetToken.Password, resetToken.ConfirmPassword) != 0)
            {
                return BadRequest("Password and ConfirmPassword doesnot match! ");
            }
            if (string.IsNullOrEmpty(resetToken.Token))
            {
                return BadRequest("Invalid Token! ");
            }
            var result = await _userManager.ResetPasswordAsync(user, resetToken.Token, resetToken.Password);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return StatusCode(500, result.Errors);
            }
            return Ok(new UserDto
            {
                Email = resetToken.Email,
                Username = resetToken.Username,
                Password = resetToken.Password,
                ConfirmPassword = resetToken.ConfirmPassword,
                Token = resetToken.Token,
            });
        }

        [Authorize(Roles = "Owner, Staff, Customer")]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePassword)
        {
            var user = await _userManager.FindByNameAsync(changePassword.UserName);
            if (user == null)
            {
                return BadRequest("User Not Exist");
            }
            if (string.Compare(changePassword.NewPassword, changePassword.ConfirmNewPassword) != 0)
            {
                return BadRequest("Password and ConfirmPassword doesnot match! ");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePassword.CurrentPassword, changePassword.NewPassword);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                return StatusCode(500, result.Errors);
            }
            return Ok(new UserDto
            {
                Username = changePassword.UserName,
                Password = changePassword.NewPassword,
                ConfirmPassword = changePassword.ConfirmNewPassword
            });
        }
    }
}
