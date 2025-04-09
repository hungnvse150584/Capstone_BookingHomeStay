using BusinessObject.Model;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;
using Repository.IRepositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class AISuggestionService : IAISuggestionService
    {
        private readonly IHomeStayRepository _homeStayRepository;
        private readonly SessionsClient _sessionsClient;
        private readonly string _projectId = "homestaychatbot-jmhc"; // Cập nhật Project ID của bạn
        private readonly string _sessionIdPrefix = "greenroam-session";

        public AISuggestionService(IHomeStayRepository homeStayRepository)
        {
            _homeStayRepository = homeStayRepository;

            // Khởi tạo Dialogflow client với file credentials
            var credentialsPath = @"C:\Users\84846\Downloads\homestaychatbot-jmhc-e3c3740297d6.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);
            _sessionsClient = SessionsClient.Create();
        }

        public async Task<List<string>> GetInitialSuggestionsAsync(int homeStayId)
        {
            var homeStay = await _homeStayRepository.GetByIdAsync(homeStayId);
            if (homeStay == null) throw new Exception("HomeStay not found.");

            var sessionId = $"{_sessionIdPrefix}-{homeStayId}";
            var session = new SessionName(_projectId, sessionId);

            var queryInput = new QueryInput
            {
                Event = new EventInput
                {
                    Name = "UnderstandCustomerNeeds",
                    LanguageCode = "vi"
                }
            };

            var response = await _sessionsClient.DetectIntentAsync(session, queryInput);
            return CustomizeSuggestions(response.QueryResult.FulfillmentMessages, homeStay);
        }

        public async Task<List<string>> GetDetailedSuggestionsAsync(string customerMessage, int homeStayId)
        {
            var homeStay = await _homeStayRepository.GetByIdAsync(homeStayId);
            if (homeStay == null) throw new Exception("HomeStay not found.");

            var sessionId = $"{_sessionIdPrefix}-{homeStayId}";
            var session = new SessionName(_projectId, sessionId);

            var queryInput = new QueryInput
            {
                Text = new TextInput
                {
                    Text = customerMessage,
                    LanguageCode = "vi"
                }
            };

            var response = await _sessionsClient.DetectIntentAsync(session, queryInput);
            if (response.QueryResult.Intent.DisplayName == "AskForMoreDetails")
            {
                return CustomizeSuggestions(response.QueryResult.FulfillmentMessages, homeStay);
            }
            return new List<string>(); // Trả về rỗng nếu không match intent
        }

        private List<string> CustomizeSuggestions(IEnumerable<Google.Cloud.Dialogflow.V2.Intent.Types.Message> messages, HomeStay homeStay)
        {
            var suggestions = new List<string>();
            foreach (var message in messages)
            {
                var text = message.Text.Text_[0]; // Lấy text đầu tiên từ FulfillmentMessages
                // Tùy chỉnh gợi ý dựa trên thông tin homestay
                if (text.Contains("[giá]"))
                    text = text.Replace("[giá]", "500,000 VND"); // Ví dụ: thay giá cụ thể từ dữ liệu homestay
                if (text.Contains("phòng phù hợp") && homeStay.TypeOfRental == RentalType.Apartment)
                    text = "Bạn có muốn biết về các căn hộ phù hợp cho nhóm của bạn không?";
                if (text.Contains("dịch vụ bữa sáng") && homeStay.Services?.Any(s => s.servicesName.Contains("bữa sáng")) == true)
                    text = "Bạn có muốn biết thêm về dịch vụ bữa sáng không?";
                suggestions.Add(text);
            }
            return suggestions;
        }
    }
}