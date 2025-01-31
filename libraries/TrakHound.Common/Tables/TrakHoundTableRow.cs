namespace TrakHound.Tables
{
    public struct TrakHoundTableRow
    {
        public string Uuid { get; set; }

        public string Key { get; set; }

        public long Created { get; set; }

        public object[] Cells { get; set; }


        public TrakHoundTableRow() { }

        public TrakHoundTableRow(string key, object[] cells, long created = 0)
        {
            Key = key;
            Cells = cells;
            Created = created > 0 ? created : UnixDateTime.Now;
        }
    }
}
