namespace Oculus.Platform.Models
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    /// It represents an error response from the server.
    /// It contains error information such as the error code, http status code, technical message, and displayable message.
    /// It's used whenever the server needs to communicate an error or failure to the client.
    public class Error
    {
        public Error(int code, string message, int httpCode)
        {
            Message = message;
            Code = code;
            HttpCode = httpCode;
        }
        /// It's a type of `integer` that represents the error code:
        /// UNKNOWN_ERROR:1
        /// AUTHENTICATION_ERROR:2
        /// NETWORK_ERROR:3
        /// STORE_INSTALLATION_ERROR:4
        /// CALLER_NOT_SIGNED:5
        /// UNKNOWN_SERVER_ERROR:6
        /// PERMISSIONS_FAILURE:7
        public readonly int Code;
        /// It contains the HTTP status code for the error. More information about the http code can be found [here](https://en.wikipedia.org/wiki/List_of_HTTP_status_codes).
        public readonly int HttpCode;
        /// Technical description of what went wrong intended for developers. For use in logs or developer consoles.
        public readonly string Message;
    }
}
