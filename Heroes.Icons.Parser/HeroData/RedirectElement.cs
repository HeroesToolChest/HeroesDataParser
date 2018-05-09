namespace Heroes.Icons.Parser.HeroData
{
    public class RedirectElement
    {
        public RedirectElement(string id, string value)
        {
            Id = id;
            Value = value;
        }

        /// <summary>
        /// Name of the element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Redirect id name
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Override current value
        /// </summary>
        public string Value { get; }

        public RedirectElement InnerElement { get; set; }
    }
}
