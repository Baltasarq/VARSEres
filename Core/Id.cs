namespace VARSEres.Core
{
    /// <summary>
    /// Represents an identifier.
    /// </summary>
    public class Id
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:VARSEres.Core.Id"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        public Id(long id)
        {
            this.Value = id;
        }

        /// <summary>
        /// Gets the value of the identifier itself.
        /// </summary>
        /// <value>The value, as a long.</value>
        public long Value {
            get; private set;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:VARSEres.Core.Id"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:VARSEres.Core.Id"/>.</returns>
        public override string ToString()
        {
            return System.Convert.ToString( this.Value );
        }
    }
}
