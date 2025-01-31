namespace TrakHound.Tables
{
    public class TrakHoundTableResponse
    {
        public string TableUuid { get; set; }

        public TrakHoundTableColumn[] Columns { get; set; }
    }
}
