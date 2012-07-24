namespace System.Data.Entity.ModelConfiguration.Utilities
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    internal class AttributeProvider
    {
        public virtual IEnumerable<Attribute> GetAttributes(MemberInfo memberInfo)
        {
            Contract.Requires(memberInfo != null);

            var type = memberInfo as Type;

            if (type != null)
            {
                return GetAttributes(type);
            }

            return GetAttributes((PropertyInfo)memberInfo);
        }

        public virtual IEnumerable<Attribute> GetAttributes(Type type)
        {
            Contract.Requires(type != null);

            var attrs = new HashSet<Attribute>(GetTypeDescriptor(type).GetAttributes().Cast<Attribute>());

            // Data Services workaround
            foreach (var attribute in type.GetCustomAttributes(true).Cast<Attribute>()
                .Where(
                    a =>
                    a.GetType().FullName.Equals(
                        "System.Data.Services.Common.EntityPropertyMappingAttribute", StringComparison.Ordinal) &&
                    !attrs.Contains(a)))
            {
                attrs.Add(attribute);
            }

            return attrs;
        }

        public virtual IEnumerable<Attribute> GetAttributes(PropertyInfo propertyInfo)
        {
            Contract.Requires(propertyInfo != null);

            var typeDescriptor = GetTypeDescriptor(propertyInfo.DeclaringType);
            var propertyCollection = typeDescriptor.GetProperties();
            var propertyDescriptor = propertyCollection[propertyInfo.Name];

            var propertyAttributes
                = (propertyDescriptor != null)
                      ? propertyDescriptor.Attributes.Cast<Attribute>()
                  // Fallback to standard reflection (non-public properties)
                      : propertyInfo.GetCustomAttributes(true).Cast<Attribute>();

            // Get the attributes for the property's type and exclude them
            var propertyTypeAttributes = GetAttributes(propertyInfo.PropertyType);

            return propertyAttributes.Except(propertyTypeAttributes);
        }

        private static ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            Contract.Requires(type != null);

            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }
    }
}
