namespace PSEngagementSystem
{
    public class CheckIn
    {
        public int ID { get; set; }
        public int StudentID { get; set; }  
        public int Mood { get; set; }       
        public DateTime Date { get; set; }
        public string Comment { get; set; } = "";  // allows optional comments
    }
}
