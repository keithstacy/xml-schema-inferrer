#nullable enable
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using InspectionResultsDataContext = ImageDataAccess.InspectionResultsContext.InspectionResultsDataContext;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace FoundDefect.Controllers
{
    [Route("api/founddefects")]
    [ApiController]
    public class FoundDefectController : ControllerBase
    {
        private const string SqlMinDateValue = "1753-01-01";
        internal DateTime SqlMinDate { get; } = DateTime.Parse(SqlMinDateValue, CultureInfo.InvariantCulture);

        public FoundDefectController(InspectionResultsDataContext context)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            Context = context;
        }

        public InspectionResultsDataContext Context { get; set; }

        [Route("")]
        [Route("all")]
        [HttpGet]
        public IActionResult GetAllUntransmitted()
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            List<string> list;
            try
            {
                list = Context.GetFoundDefectJsonList(SqlMinDate, DateTime.Today.AddDays(1));
                return Ok(list);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, $"{InspectionResultsDataContext.GetExceptionStack(e)}\r\n{e.StackTrace}");
            }
        }

        [Route("new")]
        [HttpGet]
        public IActionResult GetNew()
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            List<string> list;
            try
            {
                list = Context.GetFoundDefectJsonList(DateTime.Today, DateTime.Today.AddDays(1));
                return Ok(list);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, $"{InspectionResultsDataContext.GetExceptionStack(e)}\r\n{e.StackTrace}");
            }
        }

        [Route("daterange/{startDate}/{endDate}")]
        [HttpGet]
        public IActionResult GetByDateRange(DateTime startDate, DateTime endDate)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            List<string> list;
            try
            {
                list = Context.GetFoundDefectJsonList(startDate, endDate);
                return Ok(list);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, $"{InspectionResultsDataContext.GetExceptionStack(e)}\r\n{e.StackTrace}");
            }
        }

        [Route("serialnumber/{serialNumber}")]
        [HttpGet]
        public IActionResult GetBySerialNumber(string serialNumber)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            List<string> list;
            try
            {
                list = Context.GetFoundDefectJsonList(serialNumber);
                return Ok(list);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, $"{InspectionResultsDataContext.GetExceptionStack(e)}\r\n{e.StackTrace}");
            }
        }

        [Route("serialnumber/{serialNumber}/cameraid/{cameraId}")]
        [HttpGet]
        public IActionResult GetBySerialNumberAndCameraId(string serialNumber, string cameraId)
        {
            WriteToLog($"Now executing: {GetMethodName(MethodBase.GetCurrentMethod())}", EventLogEntryType.Information);
            List<string> list;
            try
            {
                list = Context.GetFoundDefectJsonList(serialNumber, cameraId);
                return Ok(list);
            }
            catch (Exception e)
            {
                WriteToLog($"Exception thrown in {GetMethodName(MethodBase.GetCurrentMethod())}.\r\n{e.Message}", EventLogEntryType.Error);
                return StatusCode(500, $"{InspectionResultsDataContext.GetExceptionStack(e)}\r\n{e.StackTrace}");
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
