using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Serilog;

namespace LetsEncrypt.ACME.Simple
{
    public class ScheduledRenewal
    {
        public DateTime Date { get; set; }
        public Target Binding { get; set; }
        public string CentralSsl { get; set; }
        public string San { get; set; }
        public string KeepExisting { get; set; }
        public string Script { get; set; }
        public string ScriptParameters { get; set; }
        public bool Warmup { get; set; }
        public AzureOptions AzureOptions { get; set; }

        public override string ToString() => $"{Binding} Renew After {Date.ToShortDateString()}";

        internal string Save()
        {
            return JsonConvert.SerializeObject(this);
        }

        internal static ScheduledRenewal Load(string renewal)
        {
            var result = JsonConvert.DeserializeObject<ScheduledRenewal>(renewal);

			if (result == null || result.Binding == null) {
                Input.WriteError($"Unable to deserialize renewal {renewal}");
                return null;
            }

            if (result.Binding.Plugin == null) {
                Input.WriteError($"Plugin {result.Binding.PluginName} not found");
                return null;
            }

            try {
                result.Binding.Plugin.Refresh(result);
            } catch (Exception ex) {
                Input.WriteWarning($"Error refreshing renewal for {result.Binding.Host} - {ex.Message}");
            }

			return result;
        }
    }
}
