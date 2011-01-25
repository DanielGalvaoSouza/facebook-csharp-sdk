namespace Facebook.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Web;

    /// <summary>
    /// Facebook canvas authorizer.
    /// </summary>
    public class CanvasAuthorizer : Authorizer
    {
        /// <summary>
        /// The facebook settings.
        /// </summary>
        private readonly IFacebookAppSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasAuthorizer"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <param name="httpContext">
        /// The http context.
        /// </param>
        public CanvasAuthorizer(IFacebookAppSettings settings, HttpContextBase httpContext)
            : base(settings.AppId, settings.AppSecret, httpContext)
        {
            Contract.Requires(settings != null);
            Contract.Requires(!string.IsNullOrEmpty(settings.AppId));
            Contract.Requires(!string.IsNullOrEmpty(settings.AppSecret));
            Contract.Requires(httpContext != null);
            Contract.Requires(httpContext.Request != null);
            Contract.Requires(httpContext.Request.Params != null);

            this.settings = settings;
        }

        /// <summary>
        /// Gets the Facebook application settings.
        /// </summary>
        public IFacebookAppSettings Settings
        {
            get { return this.settings; }
        }

        /// <summary>
        /// Handle unauthorized requests.
        /// </summary>
        public override void HandleUnauthorizedRequest()
        {
            this.HttpResponse.ContentType = "text/html";
            this.HttpResponse.Write(CanvasUrlBuilder.GetCanvasRedirectHtml(this.GetLoginUrl(null)));
        }

        public Uri GetLoginUrl(IDictionary<string, object> parameters)
        {
            Contract.Ensures(Contract.Result<Uri>() != null);

            var defaultParameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(this.LoginDisplayMode))
            {
                defaultParameters["display"] = this.LoginDisplayMode;
            }

            if (!string.IsNullOrEmpty(this.Perms))
            {
                defaultParameters["scope"] = this.Perms;
            }

            var canvasUrlBuilder = new CanvasUrlBuilder(this.Settings, this.HttpRequest);
            return canvasUrlBuilder.GetLoginUrl(this.ReturnUrlPath, this.CancelUrlPath, this.State, FacebookUtils.Merge(defaultParameters, parameters));
        }

        [ContractInvariantMethod]
        private void InvarientObject()
        {
            Contract.Invariant(this.settings != null);
        }
    }
}