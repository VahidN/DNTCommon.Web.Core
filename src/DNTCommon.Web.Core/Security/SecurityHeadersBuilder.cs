using Microsoft.AspNetCore.Builder;

namespace DNTCommon.Web.Core;

/// <summary>
///     Defines the policies to use for customizing security headers for a request.
/// </summary>
public static class SecurityHeadersBuilder
{
    private static HeaderPolicyCollection? _policy;

    /// <summary>
    ///     Defines the policies to use for customizing security headers for a request.
    /// </summary>
    /// <param name="isDevelopment"></param>
    /// <param name="enableCrossOriginPolicy">
    ///     Note: enabling it will prevent us from using images and icons from the other sites
    /// </param>
    /// <returns></returns>
    public static HeaderPolicyCollection GetCsp(bool isDevelopment, bool enableCrossOriginPolicy)
    {
        if (_policy is not null)
        {
            return _policy;
        }

        _policy = new HeaderPolicyCollection().AddFrameOptionsDeny()
            .AddXssProtectionBlock()
            .AddContentTypeOptionsNoSniff()
            .EnableCrossOriginPolicy(enableCrossOriginPolicy)
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddBaseUri().Self();
                builder.AddDefaultSrc().Self().From(uri: "blob:");
                builder.AddObjectSrc().Self().From(uri: "blob:");
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().Self().From(uri: "data:").From(uri: "blob:").From(uri: "https:");
                builder.AddFontSrc().Self();

                builder.AddStyleSrc().UnsafeInline().Self();

                builder.AddFrameAncestors().None();
                builder.AddFrameSrc().Self();
                builder.AddConnectSrc().Self();
                builder.AddMediaSrc().Self();

                builder.AddScriptSrc().Self();

                // NOTE: it's part of the DNTCommon.Web.Core\Middlewares\CspReportController.cs
                builder.AddReportUri().To(uri: "/api/CspReport/Log");

                if (!isDevelopment)
                {
                    builder.AddUpgradeInsecureRequests();
                }
            })
            .RemoveServerHeader()
            .AddPermissionsPolicy(builder =>
            {
                builder.AddAccelerometer().None();
                builder.AddAutoplay().None();
                builder.AddCamera().None();
                builder.AddEncryptedMedia().None();
                builder.AddFullscreen().All();
                builder.AddGeolocation().None();
                builder.AddGyroscope().None();
                builder.AddMagnetometer().None();
                builder.AddMicrophone().None();
                builder.AddMidi().None();
                builder.AddPayment().None();
                builder.AddPictureInPicture().None();
                builder.AddSyncXHR().None();
                builder.AddUsb().None();
            });

        if (!isDevelopment)
        {
            // Default maxAge => one year in seconds
            _policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains();
        }

        _policy.ApplyDocumentHeadersToAllResponses();

        return _policy;
    }

    /// <summary>
    ///     Note: enabling it will prevent us from using images and icons from the other sites
    /// </summary>
    private static HeaderPolicyCollection EnableCrossOriginPolicy(this HeaderPolicyCollection policies, bool enable)
        => enable
            ? policies.AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
                .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
                .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp())
            : policies;
}