﻿// Copyright (c) 2022 Pilz GmbH & Co. KG
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice (including the next paragraph) shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// SPDX-License-Identifier: MIT

using System;
using System.Diagnostics;
using System.Net;

namespace Pilz.PITreader.Client.Model
{
    /// <summary>
    /// Response object returned on API requests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ApiResponse<T> where T : class
    {
        /// <summary>
        /// The request was executed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Request can be retried.
        /// </summary>
        public bool Retryable { get; set; }

        /// <summary>
        /// Response code returned by the PITreader.
        /// </summary>
        public ResponseCode ResponseCode { get; set; }

        /// <summary>
        /// The Data element is filled with the response on successfull requests.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// The ErrorData element is filled with the response on not successfull requests.
        /// It is always of type GenericResponse.
        /// </summary>
        public GenericResponse ErrorData { get; set; }

        private string DebuggerDisplay { get => string.Format("Succes: {0} ({1}), Data: {{ {2} }}", this.Success, this.ResponseCode, (this.Success ? (object)this.Data : (object)this.ErrorData) ?? "null"); }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => this.DebuggerDisplay;

        /// <summary>
        /// Returns new ApiResponse of type GenericResponse with ErrorData of current response object.
        /// </summary>
        /// <returns>New ApiResponse of type GenericResponse with ErrorData of current response object.</returns>
        public ApiResponse<GenericResponse> AsGenericResponse()
        {
            return new ApiResponse<GenericResponse>
            {
                Data = new GenericResponse { Success = this.Success, Message = this.ErrorData?.Message, Data = this.ErrorData?.Data },
                ErrorData = this.ErrorData,
                Success = this.Success,
                ResponseCode = this.ResponseCode,
                Retryable = this.Retryable
            };
        }

        /// <summary>
        /// Creates a new ApiResponse{T} instance on successful requests.
        /// </summary>
        /// <param name="data">Response data object</param>
        /// <returns></returns>
        public static ApiResponse<T> Ok(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Retryable = false,
                ResponseCode = ResponseCode.Ok,
                Data = data
            };
        }

        /// <summary>
        /// Creates a new ApiResponse{T} instance on errors.
        /// </summary>
        /// <param name="statusCode">HTTP Status code of response.</param>
        /// <param name="retry">Possibility to retry request.</param>
        /// <param name="data">Response data object</param>
        /// <returns></returns>
        public static ApiResponse<T> Error(HttpStatusCode statusCode, bool retry, GenericResponse data)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Retryable = retry,
                ResponseCode = Enum.IsDefined(typeof(ResponseCode), ((int)statusCode)) ? (ResponseCode)statusCode : ResponseCode.Unknown,
                ErrorData = data
            };
        }

        /// <summary>
        /// Creates a new ApiResponse{T} instance on errors.
        /// </summary>
        /// <param name="message">Message text.</param>
        /// <param name="retry">Possibility to retry request.</param>
        /// <param name="code">Response code.</param>
        /// <returns></returns>
        public static ApiResponse<T> Error(string message, bool retry, ResponseCode? code = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Retryable = retry,
                ResponseCode = code ?? ResponseCode.Unknown,
                ErrorData = new GenericResponse { Success = false, Message = message }
            };
        }
    }
}