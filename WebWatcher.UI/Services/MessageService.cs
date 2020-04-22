using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebWatcher.UI.Interfaces;
using WebWatcher.UI.Models;

namespace WebWatcher.UI.Services
{
    public class MessageService : IMessageService
    {
        protected readonly string _schema;
        protected readonly string _command;
        protected readonly string _error;
        protected Dictionary<string, int> _colors;
        public MessageService()
        {
            _colors = new Dictionary<string, int>();
            _colors.Add("green", 2);
            _colors.Add("red", 3);
            _colors.Add("yellow", 4);

            _schema = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[{{TIME}}] " +
                    @"\cf1 [{{URL}}] " +
                    @"\cf{{COLORNUM}} \b [{{STATUS}}] \b0" +
                    @"\par}";

            _command = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[{{TIME}}] " +
                    @"\cf1 [{{COMMAND}}] " +
                    @"\cf1 \b [{{MESSAGE}}] \b0" +
                    @"\par}";

            _error = @"{\rtf1\pc" +
                    @"{\colortbl;\red255\green255\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue0;}" +
                    @"\cf1[{{TIME}}] " +
                    @"\cf1 [Internal error] " +
                    @"\cf1 \b [{{MESSAGE}}] \b0" +
                    @"\par}";
        }

        public Result<string> ResponseToText(Response log)
        {
            if (log is null)
            {
                var e = new ArgumentNullException(nameof(log));
                return new Result<string>(null, e);
            }

            var text = (string)_schema.Clone();

            text = text.Replace("{{TIME}}", log.DateTime.ToLongTimeString())
                .Replace("{{URL}}", log.Url)
                .Replace("{{STATUS}}", log.Status.ToString());

            switch (log.State)
            {
                case State.Ok:
                    text = text.Replace("{{COLORNUM}}", _colors["green"].ToString());
                    break;
                case State.Error:
                    text = text.Replace("{{COLORNUM}}", _colors["red"].ToString());
                    break;
                case State.Warning:
                default:
                    text = text.Replace("{{COLORNUM}}", _colors["yellow"].ToString());
                    break;
            }

            return new Result<string>(text);
        }

        public string CommandToText(Command command)
        {
            var output = (string)_command.Clone();
            output = output
                .Replace("{{TIME}}", DateTime.Now.ToLongTimeString())
                .Replace("{{COMMAND}}", $"Timer {command.ToString()}");

            switch (command)
            {
                case Command.Reset:
                    output = output.Replace("{{MESSAGE}}", "---Timer resetted by user---");
                    break;
                case Command.Start:
                    output = output.Replace("{{MESSAGE}}", "---Timer on---");
                    break;
                case Command.Stop:
                    output = output.Replace("{{MESSAGE}}", "---Timer off---");
                    break;
                default:
                    output = string.Empty;
                    break;
            }

            return output;
        }

        public string GetError(string message = "An internal error occured.")
        {
            var output = (string)_error.Clone();
            output = output
                .Replace("{{TIME}}", DateTime.Now.ToLongTimeString())
                .Replace("{{MESSAGE}}", message);

            return output;
        }
    }
}
