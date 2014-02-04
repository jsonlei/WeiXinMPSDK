﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Senparc.Weixin.MP.Helpers;

namespace Senparc.Weixin.MP.Entities
{
    public interface IResponseMessageBase : IMessageBase
    {
        ResponseMsgType MsgType { get; }
        //string Content { get; set; }
        bool FuncFlag { get; set; }
    }

    /// <summary>
    /// 响应回复消息
    /// </summary>
    public class ResponseMessageBase : MessageBase, IResponseMessageBase
    {
        public virtual ResponseMsgType MsgType
        {
            get { return ResponseMsgType.Text; }
        }
        //public string Content { get; set; }
        public bool FuncFlag { get; set; }

        /// <summary>
        /// 获取响应类型实例，并初始化
        /// </summary>
        /// <param name="requestMessage">请求</param>
        /// <param name="msgType">响应类型</param>
        /// <returns></returns>
        [Obsolete("建议使用CreateFromRequestMessage<T>(IRequestMessageBase requestMessage)取代此方法")]
        public static ResponseMessageBase CreateFromRequestMessage(IRequestMessageBase requestMessage, ResponseMsgType msgType)
        {
            ResponseMessageBase responseMessage = null;
            try
            {
                switch (msgType)
                {
                    case ResponseMsgType.Text:
                        responseMessage = new ResponseMessageText();
                        break;
                    case ResponseMsgType.News:
                        responseMessage = new ResponseMessageNews();
                        break;
                    case ResponseMsgType.Music:
                        responseMessage = new ResponseMessageMusic();
                        break;
                    case ResponseMsgType.Image:
                        responseMessage = new ResponseMessageImage();
                        break;
                    case ResponseMsgType.Voice:
                        responseMessage = new ResponseMessageVoice();
                        break;
                    case ResponseMsgType.Video:
                        responseMessage = new ResponseMessageVideo();
                        break;
                    default:
                        throw new UnknownRequestMsgTypeException(string.Format("ResponseMsgType没有为 {0} 提供对应处理程序。", msgType), new ArgumentOutOfRangeException());
                }

                responseMessage.ToUserName = requestMessage.FromUserName;
                responseMessage.FromUserName = requestMessage.ToUserName;
                responseMessage.CreateTime = DateTime.Now; //使用当前最新时间

            }
            catch (Exception ex)
            {
                throw new WeixinException("CreateFromRequestMessage过程发生异常", ex);
            }

            return responseMessage;
        }

        /// <summary>
        /// 获取响应类型实例，并初始化
        /// </summary>
        /// <typeparam name="T">需要返回的类型</typeparam>
        /// <param name="requestMessage">请求数据</param>
        /// <returns></returns>
        public static T CreateFromRequestMessage<T>(IRequestMessageBase requestMessage) where T : ResponseMessageBase
        {
            try
            {
                var tType = typeof(T);
                var responseName = tType.Name.Replace("ResponseMessage", ""); //请求名称
                ResponseMsgType msgType = (ResponseMsgType)Enum.Parse(typeof(ResponseMsgType), responseName);
                return CreateFromRequestMessage(requestMessage, msgType) as T;
            }
            catch (Exception ex)
            {
                throw new WeixinException("ResponseMessageBase.CreateFromRequestMessage<T>过程发生异常！", ex);
            }
        }

        /// <summary>
        /// 从返回结果XML转换成IResponseMessageBase实体类
        /// </summary>
        /// <param name="xml">返回给服务器的Response Xml</param>
        /// <returns></returns>
        public static IResponseMessageBase CreateFromResponseXml(string xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml))
                {
                    return null;
                }

                var doc = XDocument.Parse(xml);
                ResponseMessageBase responseMessage = null;
                var msgType = (ResponseMsgType)Enum.Parse(typeof(ResponseMsgType), doc.Root.Element("MsgType").Value, true);
                switch (msgType)
                {
                    case ResponseMsgType.Text:
                        responseMessage = new ResponseMessageText();
                        break;
                    case ResponseMsgType.News:
                        responseMessage = new ResponseMessageNews();
                        break;
                    case ResponseMsgType.Music:
                        responseMessage = new ResponseMessageMusic();
                        break;
                }

                responseMessage.FillEntityWithXml(doc);
                return responseMessage;
            }
            catch (Exception ex)
            {
                throw new WeixinException("ResponseMessageBase.CreateFromResponseXml<T>过程发生异常！" + ex.Message, ex);
            }
        }
    }
}
