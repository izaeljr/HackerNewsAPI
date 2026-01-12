namespace HackerNewsAPI.Models
{
    public class StoryResponse
    {
        public string Title { get; set; }
        public string Uri { get; set; }
        public string PostedBy { get; set; }
        public DateTimeOffset Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }

        public StoryResponse(string title, string uri, string postedBy,
            DateTimeOffset time, int score, int commentCount) 
        {
            Title = title;
            Uri = uri;
            PostedBy = postedBy;
            Time = time;
            Score = score;
            CommentCount = commentCount;
        }
    }
}
