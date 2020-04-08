using System.Collections.Generic;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData
{
    public class XmlArrayElement
    {
        private readonly Dictionary<int, XElement> _xElementByIndex = new Dictionary<int, XElement>();

        /// <summary>
        /// Gets the maximum index value of the array collection.
        /// </summary>
        public int MaxIndex { get; private set; } = 0;

        /// <summary>
        /// Gets a collection of the elements from the array.
        /// </summary>
        public IEnumerable<XElement> Elements => _xElementByIndex.Values;

        /// <summary>
        /// Adds an element to the array collection.
        /// </summary>
        /// <param name="element">The <see cref="XElement"/> to be added.</param>
        public void AddElement(XElement element)
        {
            if (element is null)
                throw new System.ArgumentNullException(nameof(element));

            string? indexValue = element.Attribute("index")?.Value ?? element.Element("index")?.Attribute("value")?.Value;
            string? removedValue = element.Attribute("removed")?.Value ?? element.Element("removed")?.Attribute("value")?.Value;

            if (int.TryParse(indexValue, out int indexResult) && _xElementByIndex.TryGetValue(indexResult, out XElement? existingElement) && string.IsNullOrEmpty(removedValue))
            {
                foreach (XAttribute attribute in existingElement.Attributes())
                {
                    XAttribute currentAttribute = element.Attribute(attribute.Name.LocalName);
                    if (currentAttribute == null)
                        element.Add(attribute);
                    else
                        element.SetAttributeValue(attribute.Name.LocalName, currentAttribute.Value);
                }

                _xElementByIndex[indexResult] = element;
            }
            else if (int.TryParse(removedValue, out int removedResult) && removedResult == 1)
            {
                _xElementByIndex.Remove(indexResult);
            }
            else
            {
                if (_xElementByIndex.ContainsKey(MaxIndex))
                    MaxIndex++;

                _xElementByIndex.Add(MaxIndex, element);
            }
        }
    }
}
