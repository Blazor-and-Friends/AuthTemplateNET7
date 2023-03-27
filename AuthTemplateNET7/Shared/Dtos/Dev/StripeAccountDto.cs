using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;
public class StripeAccountDto : IValidatableObject
{
    public bool TestMode { get; set; } = true;
    public string TestPublishableKey { get; set; }
    public string TestSecretKey { get; set; }
    public string PublishableKey { get; set; }
    public string SecretKey { get; set; }

    #region UI

    public bool DeleteAccount { get; set; }

    #endregion //UI

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DeleteAccount) return null;

        List<ValidationResult> result = new List<ValidationResult>();
        if (TestMode)
        {
            if (string.IsNullOrWhiteSpace(TestPublishableKey))
            {
                result.Add(new ValidationResult("Can't be null in test mode", new[] { "TestPublicKey" }));
            }
            if (string.IsNullOrWhiteSpace(TestSecretKey))
            {
                result.Add(new ValidationResult("Can't be null in test mode", new[] { "TestSecretKey" }));
            }

            if (TestPublishableKey != null && !TestPublishableKey.StartsWith("pk_test"))
            {
                result.Add(new ValidationResult("Publishable test keys usually start with 'pk_test'.", new[] { "TestPublicKey" }));
            }

            if (TestSecretKey != null && !TestSecretKey.StartsWith("sk_test"))
            {
                result.Add(new ValidationResult("Secret test keys usually start with 'sk_test'.", new[] { "TestSecretKey" }));
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(PublishableKey))
            {
                result.Add(new ValidationResult("Can't be null in test mode", new[] { "PublicKey" }));
            }
            if (string.IsNullOrWhiteSpace(SecretKey))
            {
                result.Add(new ValidationResult("Can't be null in test mode", new[] { "SecretKey" }));
            }


        }

        return result;
    }
}
