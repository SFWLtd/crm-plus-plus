using System;
using Civica.CrmPlusPlus.Sdk.Validation;

namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.PropertyTypes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StringAttribute : Attribute
    {
        public int MaxLength { get; }

        public Microsoft.Xrm.Sdk.Metadata.StringFormatName StringFormat { get; }

        public StringAttribute(int maxLength, Metadata.StringFormatName stringFormat)
        {
            Guard.This(maxLength)
                .AgainstNegative()
                .AgainstZero();

            MaxLength = maxLength;

            switch (stringFormat)
            {
                case Metadata.StringFormatName.Email:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.Email;
                        return;
                case Metadata.StringFormatName.Phone:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.Phone;
                        return;
                case Metadata.StringFormatName.PhoneticGuide:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.PhoneticGuide;
                        return;
                case Metadata.StringFormatName.Text:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.Text;
                        return;
                case Metadata.StringFormatName.TextArea:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.TextArea;
                        return;
                case Metadata.StringFormatName.TickerSymbol:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.TickerSymbol;
                        return;
                case Metadata.StringFormatName.Url:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.Url;
                        return;
                case Metadata.StringFormatName.VersionNumber:
                    StringFormat = Microsoft.Xrm.Sdk.Metadata.StringFormatName.VersionNumber;
                        return;
            }
        }
    }
}
