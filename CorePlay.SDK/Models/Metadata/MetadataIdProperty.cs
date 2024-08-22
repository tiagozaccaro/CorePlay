namespace CorePlay.SDK.Models.Metadata
{
    /// <summary>
    /// Represents metadata property referencing database object by ID.
    /// </summary>
    public class MetadataIdProperty : MetadataProperty
    {
        /// <summary>
        /// Gets ID of referenced object.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Creates new instance of <see cref="MetadataIdProperty"/>.
        /// </summary>
        /// <param name="dbId"></param>
        public MetadataIdProperty(Guid dbId)
        {
            if (dbId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(dbId));
            }

            Id = dbId;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Id.ToString();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as MetadataIdProperty);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Equals(MetadataIdProperty other)
        {
            if (other == null)
            {
                return false;
            }

            return Id == other.Id;
        }
    }
}
