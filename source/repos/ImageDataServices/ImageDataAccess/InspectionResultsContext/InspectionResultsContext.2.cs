using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ImageDataAccess.InspectionResultsContext
{
    public partial class InspectionResultsDataContext
    {
        #region Constants
        private const string InsertQ4LProc = "InsertQ4LineOutputJson";
        private const string GetFoundDefectsProc = "GetFoundDefectsJson";
        private const string GetFoundDefectsBySerialAndFovProc = "GetFoundDefectsJsonBySerialAndCamera";
        private const string GetFoundDefectsBySerialProc = "GetFoundDefectsJsonBySerialNumber";
        private const string JsonParamName = "@json";
        private const string StartDateParamName = "@startdate";
        private const string EndDateParamName = "@enddate";
        private const string SerialNumberParamName = "@serialNumber";
        private const string CameraIdParamName = "@fov";

        #endregion

        public void InsertQ4LineOutputFromJson(string json)
        {

            Log.Information("Executing {0}", nameof(InsertQ4LineOutputFromJson));
            ExecuteStoredProcedure(InsertQ4LProc, null, new ValueTuple<string, string>(JsonParamName, json));
        }

        public Task InsertQ4LineOutputFromJsonAsync(string json)
        {
            Log.Information("Executing {0}", nameof(InsertQ4LineOutputFromJsonAsync));
            return ExecuteStoredProcedureAsync(InsertQ4LProc, null, new ValueTuple<string, string>(JsonParamName, json));
        }

        public List<string> GetFoundDefectJsonList(DateTime startDate, DateTime endDate)
        {
            Log.Information("Executing {0}", nameof(GetFoundDefectJsonList));
            var startDateTuple = new ValueTuple<string, DateTime>(StartDateParamName, startDate);
            var endDateTuple = new ValueTuple<string, DateTime>(EndDateParamName, endDate);
            return GetListFromStoredProcedure(GetFoundDefectsProc, null, startDateTuple, endDateTuple);
        }

        public List<string> GetFoundDefectJsonList(string serialNumber)
        {
            Log.Information("Executing {0}", nameof(GetFoundDefectJsonList));
            var tuple = new ValueTuple<string, string>(SerialNumberParamName, serialNumber);
            return GetListFromStoredProcedure(GetFoundDefectsBySerialProc,null, tuple);
        }

        public List<string> GetFoundDefectJsonList(string serialNumber, string cameraId)
        {
            Log.Information("Executing {0}", nameof(GetFoundDefectJsonList));
            var serialNumberTuple = new ValueTuple<string, string>(SerialNumberParamName, serialNumber);
            var cameraIdTuple = new ValueTuple<string, string>(CameraIdParamName, cameraId);
            return GetListFromStoredProcedure(GetFoundDefectsBySerialAndFovProc, null, serialNumberTuple, cameraIdTuple);
        }

        private List<string> GetListFromStoredProcedure(string spName, [CallerMemberName] string memberName = "", params ValueTuple<string, object>[] args)
        {
            try
            {
                if (spName != GetFoundDefectsProc &&
                    spName != InsertQ4LProc &&
                    spName != GetFoundDefectsBySerialProc &&
                    spName != GetFoundDefectsBySerialAndFovProc)
                    throw new SecurityException();
                var jsonList = new List<string>();
                using (var connection = (SqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.Database))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = spName;
                        foreach (var arg in args)
                        {
                            command.Parameters.AddWithValue(arg.Item1, arg.Item2);
                        }

                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = ((IDataRecord)reader)[0].ToString();
                            if (!string.IsNullOrWhiteSpace(row)) jsonList.Add(row);
                        }
                    }
                }
                return jsonList;
            }
            catch (SecurityException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (SqlException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
        }

        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Validation is already in place.")]
        private async Task<IEnumerable<string>> GetListFromStoredProcedureAsync(string spName, [CallerMemberName] string memberName = "", params ValueTuple<string, object>[] args)
        {
            try
            {
                if (spName != GetFoundDefectsProc &&
                    spName != InsertQ4LProc &&
                    spName != GetFoundDefectsBySerialProc)
                    throw new SecurityException();
                var jsonList = new List<string>();
                await using (var connection = (SqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.Database))
                {
                    await using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = spName;
                        foreach (var arg in args)
                        {
                            command.Parameters.AddWithValue(arg.Item1, arg.Item2);
                        }
                        await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                        var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var row = ((IDataRecord)reader)[0].ToString();
                            if (!string.IsNullOrWhiteSpace(row)) jsonList.Add(row);
                        }
                    }
                }
                return jsonList;
            }
            catch (SecurityException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (SqlException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
        }

        private void ExecuteStoredProcedure(string spName, [CallerMemberName] string memberName = "", params ValueTuple<string, object>[] args)
        {
            try
            {
                if (spName != GetFoundDefectsProc &&
                    spName != InsertQ4LProc &&
                    spName != GetFoundDefectsBySerialProc)
                {
                    throw new SecurityException();
                }

                using (var connection = (SqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.Database))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = spName;
                        foreach (var arg in args)
                        {
                            command.Parameters.AddWithValue(arg.Item1, arg.Item2);
                        }

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SecurityException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (SqlException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
        }

        private async Task ExecuteStoredProcedureAsync(string spName, [CallerMemberName] string memberName = "", params ValueTuple<string, object>[] args)
        {
            try
            {
                if (spName != GetFoundDefectsProc &&
                    spName != InsertQ4LProc &&
                    spName != GetFoundDefectsBySerialProc)
                    throw new SecurityException();
                await using (var connection = (SqlConnection)RelationalDatabaseFacadeExtensions.GetDbConnection(this.Database))
                {
                    await using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = spName;
                        foreach (var arg in args)
                        {
                            command.Parameters.AddWithValue(arg.Item1, arg.Item2);
                        }
                        await connection.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                        await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                }
            }
            catch (SecurityException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (SqlException e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
            catch (Exception e)
            {
                Log.Error(e, "{0} in call to {1}: ", e.GetType().Name, memberName);
                throw;
            }
        }

        public static string GetExceptionStack(Exception e)
        {

            var exceptionString = e.Message;
            if (e.InnerException == null)
            {
                return exceptionString;
            }

            var nextException = GetExceptionStack(e.InnerException);// yes, this is supposed to be recursive.
            exceptionString += $"\r\n\r\nInner Exception: {nextException}\r\n\r\n";

            return exceptionString;
        }

    }
}
