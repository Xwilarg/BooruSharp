using BooruSharp.Search.Tag;

namespace BooruSharp.Search.Autocomplete
{
    ///<summary>
    /// Represents an autocomplete API search result.
    ///</summary>
    public readonly struct SearchResult
    {
        /// <summary>
        /// Initializes a <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="id">The ID of the tag.</param>
        /// <param name="name">The name of the tag.</param>
        /// <param name="label">The label (display name) of the tag.</param>
        /// <param name="type">The type of the tag.</param>
        /// <param name="count">The number of occurences of the tag.</param>
        /// <param name="antecedent_name">The previous name of the tag.</param>
        public SearchResult(int? id, string name, string label, TagType? type, int count, string antecedent_name)
        {
            TagID = id;
            Label = label;
            Name = name;
            Type = type;
            Count = count;
            AntecedentName = antecedent_name;
        }

        /// <summary>
        /// Gets the ID of the tag.
        /// <see langword="null"/> if not on E621 or E926.
        /// </summary>
        public int? TagID { get; }

        /// <summary>
        /// Gets the label (display name) of the tag.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the tag.
        /// <see langword="null"/> if on GelBooru02
        /// </summary>
        /// Also known as "category".
        public TagType? Type { get; }

        /// <summary>
        /// Gets the number of occurences of the tag.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the previous name of the tag.
        /// <see langword="null"/> if not on E621 or E926.
        /// </summary> 
        public string AntecedentName { get; }
    }
}