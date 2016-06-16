namespace NahaAuto.Code
{
    public struct Status
    {
        public Status(int totalItem, int itemIndex, string message)
        {
            ItemIndex = itemIndex;
            TotalItem = totalItem;
            Message = message;
        }

        public float PercentWork => ItemIndex * 1.0f / TotalItem;

        public int ItemIndex { get; }

        public int TotalItem { get; }

        public string Message { get; }
    }
}