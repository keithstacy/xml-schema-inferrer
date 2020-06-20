using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ImageDataAccess.TuriContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Serilog;

namespace TaggedDefect.Controllers
{
    [Route(template: "api/taggeddefects")]
    [ApiController]
    public class TaggedDefectController : ControllerBase
    {
        public TaggedDefectController(TuriDataContext context)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            Context = context;
        }

        public TuriDataContext Context { get; set; }

        [Route(template: "")]
        [HttpGet]
        public IActionResult Get(string defectName = null, string taggerName = null)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            try
            {
                var taggedDefects = Context.TuriTaggedDefects.ToList();
                var filteredtaggedDefects =
                    taggedDefects.Where(predicate: x => (x.DefectName == defectName || defectName == null));
                var json = JsonConvert.SerializeObject(value: filteredtaggedDefects, formatting: Formatting.Indented);
                return Ok(value: json);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500,
                    $"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}");
            }
        }

        [Route(template: "DefectNames")]
        [HttpGet]
        public IActionResult GetDefectNames()
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            try
            {
                var taggedDefects = Context.TuriTaggedDefects.ToList();
                var defectNames = taggedDefects.Select(selector: x => x.DefectName).Distinct();
                var json = JsonConvert.SerializeObject(value: defectNames);
                return Ok(value: json);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500,
                    $"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}");
            }
        }

        [Route(template: "GetDistinct")]
        [HttpGet]
        public IActionResult GetDistinct(string columnName)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            try
            {
                if (!string.IsNullOrEmpty(value: columnName))
                {
                    var taggedDefectType = typeof(TuriTaggedDefect);
                    var propertyInfo = taggedDefectType.GetProperties();
                    var property = taggedDefectType.GetProperty(name: columnName);

                    if (property != null)
                    {
                        ////PropertyInfo property = propertyInfo.Where(x => x.Name == columnName).FirstOrDefault();
                        var taggedDefects = Context.TuriTaggedDefects.ToList();
                        var distinct = taggedDefects.Select(selector: x => property.GetValue(obj: x).ToString()).Distinct();
                        var json = JsonConvert.SerializeObject(value: distinct);
                        return Ok(value: json);
                    }
                    else
                    {
                        return StatusCode(statusCode: 500,
                            value: string.Format(provider: CultureInfo.InvariantCulture,
                                format: "{0} is not a property of TuriTaggedDefect", arg0: columnName));
                    }
                }
                else
                {
                    return BadRequest(error: "No Column Name Specified");
                }
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}",
                    EventLogEntryType.Error);
                return StatusCode(500,
                    $"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}");
            }
        }

        [Route(template: "ZeissPartIds")]
        [HttpGet]
        public IActionResult GetZeissPartIds(string defectName = null)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            try
            {
                var taggedDefects = Context.TuriTaggedDefects.ToList();
                var filteredZeissPartIds = Context.TuriTaggedDefects.Where(predicate: x => (x.DefectName == defectName || defectName == null)).Select(selector: x => x.ZeissPartId).Distinct();
                var json = JsonConvert.SerializeObject(value: filteredZeissPartIds);
                return Ok(value: json);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500,
                    $"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}");
            }
        }

        [Route(template: "post")]
        [Route(template: "")]
        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(category: "Design", checkId: "CA1031:Do not catch general exception types", Justification = "Need a catch-all for unanticipated errors.")]
        public IActionResult Post([FromBody] string value)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            if (Context == null) throw new NullReferenceException();
            try
            {
                Context.InsertTaggedDefectsFromJson(json: value);
                return Ok();
            }
            catch (JsonReaderException e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return BadRequest(error: e.Message);
            }
            catch (SqlException e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return BadRequest(error: e.Message);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(statusCode: 500, value: e.Message);
            }
        }

        private void WriteToLog(string message, EventLogEntryType type)
        {
            if (type == EventLogEntryType.Error)
            {
                Log.Error("Q4LineOutputController: {0}", message);
            }
            else
            {
                Log.Information("Q4LineOutputController: {0}", message);
            }

            EventLog.WriteEntry("Application", message, type, 5050);
        }
        private string GetMethodName(MethodBase? method)
        {
            return $"{method?.DeclaringType?.FullName}.{method?.Name}";
        }

    }
}
