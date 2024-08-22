namespace CorePlay.SDK.Models.Metadata
{
    /// <summary>
    /// Represents metadata property referencing data by name.
    /// </summary>
    public class MetadataNameProperty : MetadataProperty, IEquatable<MetadataNameProperty>
    {
        /// <summary>
        /// Property name value.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates new instance of <see cref="MetadataNameProperty"/>.
        /// </summary>
        /// <param name="name"></param>
        public MetadataNameProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as MetadataNameProperty);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Equals(MetadataNameProperty other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }
    }
}
