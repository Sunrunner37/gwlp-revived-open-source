using System;
using System.Collections.Generic;

namespace ServerEngine.Tools
{
        /// <summary>
        ///   A type used with multi key dictionaries
        /// </summary>
        /// <typeparam name="TIdentifierType">
        ///   An Enum that specifies identifiers for each unique object that IdentifierKeyEnumeration has.
        /// </typeparam>
        public interface IIdentifiable<TIdentifierType> where TIdentifierType: struct, IComparable, IFormattable, IConvertible
        {
                /// <summary>
                ///   This may be filled with and identifier type and the identifier. E.g. key=SomeEnum.NetID value=1
                /// </summary>
                IEnumerable<KeyValuePair<TIdentifierType, object>> IdentifierKeyEnumeration { get; }

                object this[TIdentifierType identType] { get; }
        }
}
