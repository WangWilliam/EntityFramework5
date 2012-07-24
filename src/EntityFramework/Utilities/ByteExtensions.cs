﻿namespace System.Data.Entity.Utilities
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;

    internal static class ByteExtensions
    {
        public static string ToHexString(this IEnumerable<byte> bytes)
        {
            Contract.Requires(bytes != null);

            var stringBuilder = new StringBuilder();

            foreach (var @byte in bytes)
            {
                stringBuilder.Append(@byte.ToString("X2", CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString();
        }
    }
}
