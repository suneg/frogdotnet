using System;
using System.Data;

namespace Frog.Orm
{
    internal class JustInTimeDataReader : IDataReader
    {
        private readonly IDbCommand command;
        private IDataReader dataReader;

        public JustInTimeDataReader(IDbCommand command)
        {
            this.command = command;
        }

        private void PrepareReader()
        {
            if(dataReader == null)
                dataReader = command.ExecuteReader();
        }

        public void Dispose()
        {
            PrepareReader();
            dataReader.Dispose();
        }

        public string GetName(int i)
        {
            PrepareReader();
            return dataReader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            PrepareReader();
            return dataReader.GetDataTypeName(i);
        }

        public Type GetFieldType(int i)
        {
            PrepareReader();
            return dataReader.GetFieldType(i);
        }

        public object GetValue(int i)
        {
            PrepareReader();
            return dataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            PrepareReader();
            return dataReader.GetValues(values);
        }

        public int GetOrdinal(string name)
        {
            PrepareReader();
            return dataReader.GetOrdinal(name);
        }

        public bool GetBoolean(int i)
        {
            PrepareReader();
            return dataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            PrepareReader();
            return dataReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            PrepareReader();
            return dataReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            PrepareReader();
            return dataReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            PrepareReader();
            return dataReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public Guid GetGuid(int i)
        {
            PrepareReader();
            return dataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            PrepareReader();
            return dataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            PrepareReader();
            return dataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            PrepareReader();
            return dataReader.GetInt64(i);
        }

        public float GetFloat(int i)
        {
            PrepareReader();
            return dataReader.GetFloat(i);
        }

        public double GetDouble(int i)
        {
            PrepareReader();
            return dataReader.GetDouble(i);
        }

        public string GetString(int i)
        {
            PrepareReader();
            return dataReader.GetString(i);
        }

        public decimal GetDecimal(int i)
        {
            PrepareReader();
            return dataReader.GetDecimal(i);
        }

        public DateTime GetDateTime(int i)
        {
            PrepareReader();
            return dataReader.GetDateTime(i);
        }

        public IDataReader GetData(int i)
        {
            PrepareReader();
            return dataReader.GetData(i);
        }

        public bool IsDBNull(int i)
        {
            PrepareReader();
            return dataReader.IsDBNull(i);
        }

        public int FieldCount
        {
            get
            {
                PrepareReader();
                return dataReader.FieldCount;
            }
        }

        object IDataRecord.this[int i]
        {
            get 
            {
                PrepareReader();
                return dataReader[i];
            }
        }

        object IDataRecord.this[string name]
        {
            get
            {
                PrepareReader();
                return dataReader[name];
            }
        }

        public void Close()
        {
            PrepareReader();
            dataReader.Close();
        }

        public DataTable GetSchemaTable()
        {
            PrepareReader();
            return dataReader.GetSchemaTable();
        }

        public bool NextResult()
        {
            PrepareReader();
            return dataReader.NextResult();
        }

        public bool Read()
        {
            PrepareReader();
            return dataReader.Read();
        }

        public int Depth
        {
            get
            {
                PrepareReader();
                return dataReader.Depth; 
            }
        }

        public bool IsClosed
        {
            get 
            {
                PrepareReader();
                return dataReader.IsClosed;
            }
        }

        public int RecordsAffected
        {
            get
            {
                PrepareReader();
                return dataReader.RecordsAffected; 
            }
        }
    }
}