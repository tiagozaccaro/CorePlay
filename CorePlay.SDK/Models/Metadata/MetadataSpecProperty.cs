namespace CorePlay.SDK.Models.Metadata
{
    /// <summary>
    /// Represents metadata property referencing specification object by id.
    /// </summary>
    public class MetadataSpecProperty : MetadataProperty, IEquatable<MetadataSpecProperty>
    {
        /// <summary>
        /// Gets specification id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Creates new instance of <see cref="MetadataSpecProperty"/>.
        /// </summary>
        /// <param name="specId"></param>
        public MetadataSpecProperty(string specId)
        {
            if (string.IsNullOrWhiteSpace(specId))
            {
                throw new ArgumentNullException(nameof(specId));
            }

            Id = specId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as MetadataSpecProperty);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Equals(MetadataSpecProperty other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id;
        }
    }
}
