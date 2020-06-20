#nullable enable
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using InspectionResultsDataContext = ImageDataAccess.InspectionResultsContext.InspectionResultsDataContext;

namespace Q4LineOutput.Controllers
{
    [Route("api/q4lineoutput")]
    [Route("api/acquisition")]
    [ApiController]
    public class Q4LineOutputController : ControllerBase
    {
        public Q4LineOutputController(InspectionResultsDataContext context)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            Context = context;
        }

        public InspectionResultsDataContext Context { get; set; }

        [Route("post")]
        [Route("")]
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            if (Context == null) throw new NullReferenceException();
            try
            {
                Context.InsertQ4LineOutputFromJson(value);
                return Ok();
            }
            catch (JsonReaderException e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return BadRequest(e.Message);
            }
            catch (SqlException e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, e.Message);
            }
        }

        [Route("post/async")]
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] string value)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            if (Context == null) throw new NullReferenceException();
            try
            {
                await Context.InsertQ4LineOutputFromJsonAsync(value).ConfigureAwait(false);
                return Ok();
            }
            catch (JsonReaderException e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return BadRequest(e.Message);
            }
            catch (SqlException e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, e.Message);
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
