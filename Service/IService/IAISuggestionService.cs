namespace Service.IService
{
    public interface IAISuggestionService
    {
        Task<List<string>> GetInitialSuggestionsAsync(int homeStayId); // Gợi ý khi chưa có tin nhắn
        Task<List<string>> GetDetailedSuggestionsAsync(string customerMessage, int homeStayId); // Gợi ý dựa trên tin nhắn khách hàng
    }
}
