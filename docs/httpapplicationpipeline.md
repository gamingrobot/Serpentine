
# HTTPApplication Pipeline

https://docs.microsoft.com/en-us/previous-versions/bb470252%28v%3dvs.140%29#life-cycle-stages

1. Validate the request, which examines the information sent by the browser and determines whether it contains potentially malicious markup. For more information, see ValidateRequest and Script Exploits Overview.
2. Perform URL mapping, if any URLs have been configured in the UrlMappingsSection section of the Web.config file.
3. Raise the BeginRequest event.
4. Raise the AuthenticateRequest event.
5. Raise the PostAuthenticateRequest event.
6. Raise the AuthorizeRequest event.
7. Raise the PostAuthorizeRequest event.
8. Raise the ResolveRequestCache event.
9. Raise the PostResolveRequestCache event.
10. Raise the MapRequestHandler event. An appropriate handler is selected based on the file-name extension of the requested resource. The handler can be a native-code module such as the IIS 7.0 StaticFileModule or a managed-code module such as the PageHandlerFactory class (which handles .aspx files). 
11. Raise the PostMapRequestHandler event.
12. Raise the AcquireRequestState event.
13. Raise the PostAcquireRequestState event.
14. Raise the PreRequestHandlerExecute event.
15. Call the ProcessRequest method (or the asynchronous version IHttpAsyncHandler.BeginProcessRequest) of the appropriate IHttpHandler class for the request. For example, if the request is for a page, the current page instance handles the request.
16. Raise the PostRequestHandlerExecute event.
17. Raise the ReleaseRequestState event.
18. Raise the PostReleaseRequestState event.
19. Perform response filtering if the Filter property is defined.
20. Raise the UpdateRequestCache event.
21. Raise the PostUpdateRequestCache event.
22. Raise the LogRequest event.
23. Raise the PostLogRequest event.
24. Raise the EndRequest event.
25. Raise the PreSendRequestHeaders event.
26. Raise the PreSendRequestContent event.

## Important Events

1. BeginRequest
2. PreRequestHandlerExecute
3. PostRequestHandlerExecute
4. EndRequest