using System.Collections.Generic;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class XmlArrayElement
    {
        private readonly Dictionary<int, XElement> XElementByIndex = new Dictionary<int, XElement>();

        /// <summary>
        /// Gets the maximum index value of the array collection.
        /// </summary>
        public int MaxIndex { get; private set; } = 0;

        /// <summary>
        /// Gets a collection of the elements from the array.
        /// </summary>
        public IEnumerable<XElement> Elements => XElementByIndex.Values;

        /// <summary>
        /// Adds an element to the array collection.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to be added.</param>
        public void AddElement(XElement element)
        {
            string indexValue = element.Attribute("index")?.Value;
            string removedValue = element.Attribute("removed")?.Value;

            if (int.TryParse(indexValue, out int indexResult) && string.IsNullOrEmpty(removedValue))
            {
                XElementByIndex[indexResult] = element;
            }
            else if (int.TryParse(removedValue, out int removedResult) && removedResult == 1)
            {
                XElementByIndex.Remove(indexResult);
            }
            else
            {
                if (XElementByIndex.ContainsKey(MaxIndex))
                    MaxIndex++;

                XElementByIndex.Add(MaxIndex, element);
            }
        }
    }
}
