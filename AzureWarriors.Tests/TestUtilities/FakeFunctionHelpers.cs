//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.DependencyInjection;
//using System.Collections;
//using System.Collections.Immutable;
//using System.Collections.Specialized;
//using System.Net;
//using System.Reflection;
//using System.Security.Claims;
//using System.Text;

//namespace AzureWarriors.Tests.TestUtilities
//{
//    #region FunctionContext and Related Fakes

//    /// <summary>
//    /// Fully-featured fake implementation of <see cref="FunctionContext"/> for testing Azure Functions.
//    /// </summary>
//    public class FakeFunctionContext : FunctionContext
//    {
//        public FakeFunctionContext()
//        {
//            InvocationId = Guid.NewGuid().ToString();

//            // Registra o serializador do Newtonsoft.Json no ServiceProvider.
//            var services = new ServiceCollection();
//            services.AddSingleton<INewtonsoftJsonObjectSerializer>(new NewtonsoftJsonObjectSerializer());
//            InstanceServices = services.BuildServiceProvider();

//            Items = new Dictionary<object, object>();
//            Features = new FakeInvocationFeatures();
//            FunctionDefinition = new FakeFunctionDefinition();
//            FunctionId = Guid.NewGuid().ToString();
//        }

//        public override string InvocationId { get; }
//        public override IServiceProvider InstanceServices { get; set; }
//        public override TraceContext TraceContext { get; } = new FakeTraceContext();
//        public override BindingContext BindingContext { get; } = new FakeBindingContext();
//        public override RetryContext RetryContext { get; } = new FakeRetryContext();
//        public override IDictionary<object, object> Items { get; set; }
//        public override IInvocationFeatures Features { get; }
//        public override FunctionDefinition FunctionDefinition { get; }
//        public override string FunctionId { get; }
//    }

//    /// <summary>
//    /// Fake implementation of <see cref="FunctionDefinition"/> for testing purposes.
//    /// </summary>
//    public class FakeFunctionDefinition : FunctionDefinition
//    {
//        public override ImmutableArray<FunctionParameter> Parameters { get; } = ImmutableArray<FunctionParameter>.Empty;
//        public override string PathToAssembly { get; } = Assembly.GetExecutingAssembly().Location;
//        public override string EntryPoint { get; } = "FakeFunction.EntryPoint";
//        public override string Id { get; } = Guid.NewGuid().ToString();
//        public override string Name { get; } = "FakeFunction";
//        public override IImmutableDictionary<string, BindingMetadata> InputBindings { get; } = ImmutableDictionary<string, BindingMetadata>.Empty;
//        public override IImmutableDictionary<string, BindingMetadata> OutputBindings { get; } = ImmutableDictionary<string, BindingMetadata>.Empty;
//    }

//    /// <summary>
//    /// Fake implementation of <see cref="TraceContext"/> for testing.
//    /// </summary>
//    public class FakeTraceContext : TraceContext
//    {
//        public override string TraceParent { get; } = Guid.NewGuid().ToString();
//        public override string TraceState { get; } = string.Empty;
//    }

//    /// <summary>
//    /// Fake implementation of <see cref="BindingContext"/> for testing.
//    /// </summary>
//    public class FakeBindingContext : BindingContext
//    {
//        public override IReadOnlyDictionary<string, object> BindingData { get; } = ImmutableDictionary<string, object>.Empty;
//    }

//    /// <summary>
//    /// Fake implementation of <see cref="RetryContext"/> for testing.
//    /// </summary>
//    public class FakeRetryContext : RetryContext
//    {
//        public override int RetryCount { get; } = 0;
//        public override int MaxRetryCount { get; } = 0;
//    }

//    /// <summary>
//    /// Fake implementation of <see cref="IInvocationFeatures"/> for testing.
//    /// </summary>
//    public class FakeInvocationFeatures : IInvocationFeatures, IEnumerable<KeyValuePair<Type, object>>
//    {
//        private readonly Dictionary<Type, object> _features = new Dictionary<Type, object>();

//        public TFeature Get<TFeature>()
//        {
//            _features.TryGetValue(typeof(TFeature), out var feature);
//            return (TFeature)feature;
//        }

//        public void Set<TFeature>(TFeature instance)
//        {
//            _features[typeof(TFeature)] = instance;
//        }

//        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => _features.GetEnumerator();
//        IEnumerator IEnumerable.GetEnumerator() => _features.GetEnumerator();
//    }

//    #endregion

//    /// <summary>
//    /// Fake implementation of HttpRequestData for testing purposes.
//    /// </summary>
//    public class FakeHttpRequestData : HttpRequestData
//    {
//        private readonly string _body;
//        private NameValueCollection? _query;
//        private Uri uri;
//        private MemoryStream memoryStream;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FakeHttpRequestData"/> class.
//        /// </summary>
//        /// <param name="context">The function context.</param>
//        /// <param name="body">The request body as a string.</param>
//        /// <param name="url">Optional: the request URL. Defaults to "http://localhost:7071/api/".</param>
//        /// <param name="method">Optional: the HTTP method. Defaults to "POST".</param>
//        public FakeHttpRequestData(FunctionContext context, string body, Uri? url = null, string method = "POST")
//            : base(context)
//        {
//            _body = body;
//            Headers = new HttpHeadersCollection();
//            // For testing, você pode definir uma lista vazia de cookies.
//            Cookies = new List<IHttpCookie>();
//            // For testing, use an empty list of identities.
//            Identities = new List<ClaimsIdentity>();
//            Url = url ?? new Uri("http://localhost:7071/api/");
//            Method = method;
//        }

//        public FakeHttpRequestData(FunctionContext functionContext, Uri uri, MemoryStream memoryStream) : base(functionContext)
//        {
//            this.uri = uri;
//            this.memoryStream = memoryStream;
//        }

//        /// <inheritdoc/>
//        public override Stream Body => new MemoryStream(Encoding.UTF8.GetBytes(_body));

//        /// <inheritdoc/>
//        public override HttpHeadersCollection Headers { get; }

//        /// <inheritdoc/>
//        public override IReadOnlyCollection<IHttpCookie> Cookies { get; }

//        /// <inheritdoc/>
//        public override Uri Url { get; }

//        /// <inheritdoc/>
//        public override IEnumerable<ClaimsIdentity> Identities { get; }

//        /// <inheritdoc/>
//        public override string Method { get; }

//        /// <inheritdoc/>
//        public override HttpResponseData CreateResponse()
//        {
//            return new FakeHttpResponseData(FunctionContext);
//        }

//        /// <summary>
//        /// Overrides the Query property to parse the query string.
//        /// </summary>
//        public override NameValueCollection Query
//        {
//            get
//            {
//                if (_query == null)
//                {
//                    _query = ParseQuery(Url.Query);
//                }
//                return _query;
//            }
//        }

//        /// <summary>
//        /// Parses a query string into a NameValueCollection.
//        /// </summary>
//        private NameValueCollection ParseQuery(string query)
//        {
//            var nvc = new NameValueCollection();
//            if (string.IsNullOrEmpty(query))
//            {
//                return nvc;
//            }

//            // Remove the '?' if present.
//            if (query.StartsWith("?"))
//            {
//                query = query.Substring(1);
//            }

//            foreach (var pair in query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
//            {
//                var parts = pair.Split(new[] { '=' }, 2);
//                var key = Uri.UnescapeDataString(parts[0]);
//                var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
//                nvc.Add(key, value);
//            }
//            return nvc;
//        }
//    }

//    /// <summary>
//    /// Fake implementation of HttpResponseData for testing HTTP triggers in Azure Functions.
//    /// </summary>
//    public class FakeHttpResponseData : HttpResponseData
//    {
//        private MemoryStream _body = new MemoryStream();
//        private HttpHeadersCollection _headers = new HttpHeadersCollection();

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FakeHttpResponseData"/> class.
//        /// </summary>
//        /// <param name="context">The function context.</param>
//        public FakeHttpResponseData(FunctionContext context)
//            : base(context)
//        {
//            // Inicializa os cookies utilizando a implementação fake.
//            Cookies = new FakeHttpCookies();
//        }

//        /// <inheritdoc/>
//        public override HttpStatusCode StatusCode { get; set; }

//        /// <inheritdoc/>
//        public override HttpHeadersCollection Headers
//        {
//            get => _headers;
//            set => _headers = value;
//        }

//        /// <inheritdoc/>
//        public override Stream Body
//        {
//            get => _body;
//            set => _body = (MemoryStream)value;
//        }

//        /// <inheritdoc/>
//        public override HttpCookies Cookies { get; }
//    }

//    #region HttpCookies and IHttpCookie

//    /// <summary>
//    /// Fake implementation of <see cref="HttpCookies"/> for testing.
//    /// </summary>
//    public class FakeHttpCookies : HttpCookies
//    {
//        private readonly Dictionary<string, string> _cookies = new Dictionary<string, string>();

//        public override void Append(string name, string value)
//        {
//            _cookies[name] = value;
//        }

//        public override void Append(IHttpCookie cookie)
//        {
//            if (cookie != null)
//            {
//                _cookies[cookie.Name] = cookie.Value;
//            }
//        }

//        public override IHttpCookie CreateNew()
//        {
//            return new FakeHttpCookie();
//        }

//    }

//    /// <summary>
//    /// Fake implementation of <see cref="IHttpCookie"/> for testing.
//    /// </summary>
//    public class FakeHttpCookie : IHttpCookie
//    {
//        public FakeHttpCookie(
//            string name = "FakeCookie",
//            string value = "FakeValue",
//            string? domain = "localhost",
//            DateTimeOffset? expires = null,
//            bool? httpOnly = false,
//            double? maxAge = null,
//            string? path = "/",
//            SameSite sameSite = SameSite.None,
//            bool? secure = false)
//        {
//            Name = name;
//            Value = value;
//            Domain = domain;
//            Expires = expires;
//            HttpOnly = httpOnly;
//            MaxAge = maxAge;
//            Path = path;
//            SameSite = sameSite;
//            Secure = secure;
//        }

//        public string? Domain { get; }
//        public DateTimeOffset? Expires { get; }
//        public bool? HttpOnly { get; }
//        public double? MaxAge { get; }
//        public string Name { get; }
//        public string? Path { get; }
//        public SameSite SameSite { get; }
//        public bool? Secure { get; }
//        public string Value { get; }
//    }

//    #endregion
//}
