using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace EadeNegarTools.EadeNegarProductManager
{
    public class DataBaseManager
    {
        public static string ConnectionString { get; set; } = "Data Source=win5.morvahost.com,1436;Initial Catalog=eadenega_enp;Persist Security Info=True;User ID=eadenega_enp_admin;Password=~7!GJAjvsz8z6iss;";
        public DataBaseManager(string _connectionString = "Data Source=win5.morvahost.com,1436;Initial Catalog=eadenega_enp;Persist Security Info=True;User ID=eadenega_enp_admin;Password=~7!GJAjvsz8z6iss;")
        {
            ConnectionString = _connectionString;
        }
        public enum enp_Table
        {
            enp_products,
            enp_product_id_types,
            enp_product_ids,
            enp_units,
            enp_material,
            enp_parts,
            enp_machines,
            enp_processes,
            enp_manufacturing_overhead,
            enp_molds,
            enp_packing,
            enp_transportation,
            enp_bom
        }
        public struct enp_products
        {
            public const string TableName = "enp_products";
            public struct enp_product
            {
                public Int64 id;
                public string name;
                public string description;
                public List<enp_product_ids.enp_pid> ids;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_products_GetAll
                {
                    public const string name = "procedure_enp_products_GetAll";
                }
                public struct procedure_enp_products_CreateProduct
                {
                    public const string name = "procedure_enp_products_CreateProduct";
                }
                public struct procedure_enp_products_GetProduct
                {
                    public const string name = "procedure_enp_products_GetProduct";
                }
                public struct procedure_enp_products_UpdateProduct
                {
                    public const string name = "procedure_enp_products_UpdateProduct";
                }
                public struct procedure_enp_products_DeleteProduct
                {
                    public const string name = "procedure_enp_products_DeleteProduct";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_product>> GetAllAsync()
            {
                var productList = new List<enp_product>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_products_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productList.Add(new enp_product()
                            {
                                id = Convert.ToInt64(reader[columns.id]),
                                name = (string)reader[columns.name]
                            });
                        }
                    }
                }
                return productList;
            }
            public static async Task<enp_product> GetAsync(Int64 id)
            {
                var output = new enp_product();
                
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_products_GetProduct.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description.ToString()] : "";
                            if (reader[enp_product_ids.GetFullColumnName(enp_product_ids.columns.id)] != DBNull.Value)
                            {
                                if (output.ids == null)
                                    output.ids = new List<enp_product_ids.enp_pid>();
                                output.ids.Add(new enp_product_ids.enp_pid()
                                {
                                    id = Convert.ToInt64(reader[enp_product_ids.GetFullColumnName(enp_product_ids.columns.id)]),
                                    type = new enp_product_id_types.enp_pid_type()
                                    {
                                        id = Convert.ToInt64(reader[enp_product_ids.GetFullColumnName(enp_product_ids.columns.product_id_type_id)]),
                                        name = (string)reader[enp_product_id_types.GetFullColumnName(enp_product_id_types.columns.name)]
                                    },
                                    value = (string)reader[enp_product_ids.GetFullColumnName(enp_product_ids.columns.value)]
                                });
                            }
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_product> CreateAsync(enp_product product)
            {
                var output = new enp_product();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_products_CreateProduct.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = product.name;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)product.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description.ToString()] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_product> UpdateAsync(enp_product product)
            {
                var output = new enp_product();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_products_UpdateProduct.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = product.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = product.name;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)product.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description.ToString()] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_product product)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_products_DeleteProduct.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = product.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_products
        //{
        //    id,
        //    name,
        //    description
        //}
        public struct enp_product_id_types
        {
            public const string TableName = "enp_product_id_types";
            public struct enp_pid_type
            {
                public Int64 id;
                public string name;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_product_id_types_GetAll
                {
                    public const string name = "procedure_enp_product_id_types_GetAll";
                }
                public struct procedure_enp_product_id_types_GetAllCompact
                {
                    public const string name = "procedure_enp_product_id_types_GetAllCompact";
                }
                public struct procedure_enp_product_id_types_CreatePID
                {
                    public const string name = "procedure_enp_product_id_types_CreatePID";
                }
                public struct procedure_enp_product_id_types_UpdatePID
                {
                    public const string name = "procedure_enp_product_id_types_UpdatePID";
                }
                public struct procedure_enp_product_id_types_DeletePID
                {
                    public const string name = "procedure_enp_product_id_types_DeletePID";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_pid_type>> GetAllAsync()
            {
                var pidList = new List<enp_pid_type>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_product_id_types_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pidList.Add(new enp_pid_type()
                            {
                                id = Convert.ToInt64(reader[columns.id]),
                                name = (string)reader[columns.name],
                                description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : ""
                            });
                        }
                    }
                }
                return pidList;
            }
            public static async Task<List<enp_pid_type>> GetAllCompactAsync()
            {
                var pidList = new List<enp_pid_type>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_product_id_types_GetAllCompact.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pidList.Add(new enp_pid_type()
                            {
                                id = Convert.ToInt64(reader[columns.id]),
                                name = (string)reader[columns.name]
                            });
                        }
                    }
                }
                return pidList;
            }
            public static async Task<enp_pid_type> CreateAsync(enp_pid_type pid)
            {
                var output = new enp_pid_type();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_product_id_types_CreatePID.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = pid.name;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)pid.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id.ToString()]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description.ToString()] != DBNull.Value ? (string)reader[columns.description.ToString()] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_pid_type pid)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_product_id_types_DeletePID.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = pid.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task<enp_pid_type> UpdateAsync(enp_pid_type pid)
            {
                var output = new enp_pid_type();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_product_id_types_UpdatePID.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = pid.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = pid.name;
                    cmd.Parameters.AddWithValue($"@{columns.description}", SqlDbType.NVarChar).Value = (object)pid.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
        }
        //public enum enp_product_id_types
        //{
        //    id,
        //    name,
        //    description
        //}
        public struct enp_product_ids
        {
            public const string TableName = "enp_product_ids";
            public struct enp_pid
            {
                public Int64 id;
                public string value;
                public enp_product_id_types.enp_pid_type type;
                public enp_products.enp_product product;
            }
            public struct columns
            {
                public const string id = "id";
                public const string product_id = "product_id";
                public const string product_id_type_id = "product_id_type_id";
                public const string value = "value";
            }
            public struct procedures
            {
                public struct procedure_enp_product_ids_GetAll
                {
                    public const string name = "procedure_enp_product_ids_GetAll";
                }
                public struct procedure_enp_product_ids_CreatePID
                {
                    public const string name = "procedure_enp_product_ids_CreatePID";
                }
                public struct procedure_enp_product_ids_UpdatePID
                {
                    public const string name = "procedure_enp_product_ids_UpdatePID";
                }
                public struct procedure_enp_product_ids_DeletePID
                {
                    public const string name = "procedure_enp_product_ids_DeletePID";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_pid>> GetAllAsync(Int64 productID)
            {
                var pidList = new List<enp_pid>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_product_ids_GetAll.name, conn) { CommandType = CommandType.StoredProcedure, Parameters = 
                        {
                            new SqlParameter($"@{columns.product_id}", SqlDbType.BigInt){ Value = productID }
                        }
                    }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            pidList.Add(new enp_pid()
                            {
                                id = Convert.ToInt64(reader[columns.id]),
                                value = (string)reader[columns.value],
                                product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) },
                                type = new enp_product_id_types.enp_pid_type() { id = Convert.ToInt64(reader[columns.product_id_type_id]), name = (string)reader[enp_product_id_types.GetFullColumnName(enp_product_id_types.columns.name)] }
                            });
                        }
                    }
                }
                return pidList;
            }
            public static async Task<enp_pid> CreateAsync(enp_pid pid)
            {
                var output = new enp_pid();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_product_ids_CreatePID.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = pid.product.id;
                    cmd.Parameters.Add($"@{columns.product_id_type_id}", SqlDbType.BigInt).Value = pid.type.id;
                    cmd.Parameters.Add($"@{columns.value}", SqlDbType.NVarChar).Value = pid.value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id.ToString()]);
                            output.value = (string)reader[columns.value];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_pid pid)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_product_ids_DeletePID.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = pid.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task<enp_pid> UpdateAsync(enp_pid pid)
            {
                var output = new enp_pid();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_product_ids_UpdatePID.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = pid.id;
                    cmd.Parameters.Add($"@{columns.value}", SqlDbType.NVarChar).Value = pid.value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id.ToString()]);
                            output.value = (string)reader[columns.value];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.type = new enp_product_id_types.enp_pid_type() { id = Convert.ToInt64(reader[columns.product_id_type_id]), name = (string)reader[enp_product_id_types.GetFullColumnName(enp_product_id_types.columns.name)] };
                        }
                    }
                }
                return output;
            }

        }
        //public enum enp_product_ids
        //{
        //    id,
        //    product_id,
        //    product_id_type_id,
        //    value
        //}
        public struct enp_units
        {
            public const string TableName = "enp_units";
            public struct enp_unit
            {
                public Int64 id;
                public string name;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_units_GetAll
                {
                    public const string name = "procedure_enp_units_GetAll";
                }
                public struct procedure_enp_units_GetAllCompact
                {
                    public const string name = "procedure_enp_units_GetAllCompact";
                }
                public struct procedure_enp_units_CreateUnit
                {
                    public const string name = "procedure_enp_units_CreateUnit";
                }
                public struct procedure_enp_units_UpdateUnit
                {
                    public const string name = "procedure_enp_units_UpdateUnit";
                }
                public struct procedure_enp_units_DeleteUnit
                {
                    public const string name = "procedure_enp_units_DeleteUnit";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_unit>> GetAllAsync()
            {
                var unitList = new List<enp_unit>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_units_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            unitList.Add(new enp_unit()
                            {
                                id = Convert.ToInt64(reader[columns.id]),
                                name = (string)reader[columns.name],
                                description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : ""
                            });
                        }
                    }
                }
                return unitList;
            }
            public static async Task<List<enp_unit>> GetAllCompactAsync()
            {
                var unitList = new List<enp_unit>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_units_GetAllCompact.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            unitList.Add(new enp_unit()
                            {
                                id = Convert.ToInt64(reader[columns.id]),
                                name = (string)reader[columns.name]
                            });
                        }
                    }
                }
                return unitList;
            }
            public static async Task<enp_unit> CreateAsync(enp_unit unit)
            {
                var output = new enp_unit();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_units_CreateUnit.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = unit.name;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)unit.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id.ToString()]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description.ToString()] != DBNull.Value ? (string)reader[columns.description.ToString()] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_unit unit)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_units_DeleteUnit.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = unit.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task<enp_unit> UpdateAsync(enp_unit unit)
            {
                var output = new enp_unit();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_units_UpdateUnit.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = unit.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = unit.name;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)unit.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = Convert.ToInt64(reader[columns.id]);
                            output.name = (string)reader[columns.name];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
        }
        //public enum enp_units
        //{
        //    id,
        //    name,
        //    description
        //}

        //public enum enp_units_procedures
        //{
        //    procedure_enp_units_GetAll,
        //    procedure_enp_units_CreateUnit,
        //    procedure_enp_units_UpdateUnit,
        //    procedure_enp_units_DeleteUnit
        //}
        public struct enp_material
        {
            public const string TableName = "enp_material";
            public struct enp_materialItem
            {
                public Int64 id;
                public string name;
                public string tech_id;
                public Int64 price;
                public string description;
                public enp_units.enp_unit unit;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string tech_id = "tech_id";
                public const string unit_id = "unit_id";
                public const string price = "price";
                public const string description = "description";

            }
            public struct procedures
            {
                public struct procedure_enp_material_GetAll
                {
                    public const string name = "procedure_enp_material_GetAll";
                }
                public struct procedure_enp_material_CreateMaterial
                {
                    public const string name = "procedure_enp_material_CreateMaterial";
                }
                public struct procedure_enp_material_GetMaterial
                {
                    public const string name = "procedure_enp_material_GetMaterial";
                }
                public struct procedure_enp_material_UpdateMaterial
                {
                    public const string name = "procedure_enp_material_UpdateMaterial";
                }
                public struct procedure_enp_material_DeleteMaterial
                {
                    public const string name = "procedure_enp_material_DeleteMaterial";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_materialItem>> GetAllAsync()
            {
                var materialList = new List<enp_materialItem>();
                using(var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_material_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            materialList.Add(new enp_materialItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name],
                                price = Convert.ToInt64(reader[columns.price]),
                                tech_id = (string)reader[columns.tech_id]
                            });
                    }
                }
                return materialList;
            }
            public static async Task<enp_materialItem> CreateAsync(enp_materialItem materialItem)
            {
                var output = new enp_materialItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_material_CreateMaterial.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = materialItem.name;
                    cmd.Parameters.Add($"@{columns.tech_id}", SqlDbType.NVarChar).Value = materialItem.tech_id;
                    cmd.Parameters.Add($"@{columns.unit_id}", SqlDbType.BigInt).Value = materialItem.unit.id;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = materialItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)materialItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.unit.id = (Int64)reader[columns.unit_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_materialItem> GetAsync(Int64 id)
            {
                var output = new enp_materialItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_material_GetMaterial.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                            output.unit = new enp_units.enp_unit() { id = (Int64)reader[columns.unit_id], name = (string)reader[enp_units.GetFullColumnName(enp_units.columns.name)] };
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_materialItem> UpdateAsync(enp_materialItem materialItem)
            {
                var output = new enp_materialItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_material_UpdateMaterial.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = materialItem.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = materialItem.name;
                    cmd.Parameters.Add($"@{columns.tech_id}", SqlDbType.NVarChar).Value = materialItem.tech_id;
                    cmd.Parameters.Add($"@{columns.unit_id}", SqlDbType.BigInt).Value = materialItem.unit.id;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = materialItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)materialItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.unit.id = (Int64)reader[columns.unit_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_materialItem materialItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_material_DeleteMaterial.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = materialItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_material
        //{
        //    id,
        //    name,
        //    tech_id,
        //    unit_id,
        //    price,
        //    description
        //}
        public struct enp_parts
        {
            public const string TableName = "enp_parts";
            public struct enp_partItem
            {
                public Int64 id;
                public string name;
                public string tech_id;
                public Int64 price;
                public string description;
                public enp_units.enp_unit unit;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string tech_id = "tech_id";
                public const string unit_id = "unit_id";
                public const string price = "price";
                public const string description = "description";

            }
            public struct procedures
            {
                public struct procedure_enp_parts_GetAll
                {
                    public const string name = "procedure_enp_parts_GetAll";
                }
                public struct procedure_enp_parts_CreatePart
                {
                    public const string name = "procedure_enp_parts_CreatePart";
                }
                public struct procedure_enp_parts_GetPart
                {
                    public const string name = "procedure_enp_parts_GetPart";
                }
                public struct procedure_enp_parts_UpdatePart
                {
                    public const string name = "procedure_enp_parts_UpdatePart";
                }
                public struct procedure_enp_parts_DeletePart
                {
                    public const string name = "procedure_enp_parts_DeletePart";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_partItem>> GetAllAsync()
            {
                var partsList = new List<enp_partItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_parts_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            partsList.Add(new enp_partItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name],
                                price = Convert.ToInt64(reader[columns.price]),
                                tech_id = (string)reader[columns.tech_id]
                            });
                    }
                }
                return partsList;
            }
            public static async Task<enp_partItem> CreateAsync(enp_partItem partItem)
            {
                var output = new enp_partItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_parts_CreatePart.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = partItem.name;
                    cmd.Parameters.Add($"@{columns.tech_id}", SqlDbType.NVarChar).Value = partItem.tech_id;
                    cmd.Parameters.Add($"@{columns.unit_id}", SqlDbType.BigInt).Value = partItem.unit.id;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = partItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)partItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.unit.id = (Int64)reader[columns.unit_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_partItem> GetAsync(Int64 id)
            {
                var output = new enp_partItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_parts_GetPart.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                            output.unit = new enp_units.enp_unit() { id = (Int64)reader[columns.unit_id], name = (string)reader[enp_units.GetFullColumnName(enp_units.columns.name)] };
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_partItem> UpdateAsync(enp_partItem partItem)
            {
                var output = new enp_partItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_parts_UpdatePart.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = partItem.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = partItem.name;
                    cmd.Parameters.Add($"@{columns.tech_id}", SqlDbType.NVarChar).Value = partItem.tech_id;
                    cmd.Parameters.Add($"@{columns.unit_id}", SqlDbType.BigInt).Value = partItem.unit.id;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = partItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)partItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.unit.id = (Int64)reader[columns.unit_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_partItem partItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_parts_DeletePart.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = partItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_parts
        //{
        //    id,
        //    name,
        //    tech_id,
        //    unit_id,
        //    price,
        //    description
        //}
        public struct enp_machines
        {
            public const string TableName = "enp_machines";
            public struct enp_machineItem
            {
                public Int64 id;
                public string name;
                public float working_days_per_year;
                public float working_hours_daily;
                public Int64 price_per_uint;
                public float life_time;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string working_days_per_year = "working_days_per_year";
                public const string working_hours_daily = "working_hours_daily";
                public const string price_per_uint = "price_per_uint";
                public const string life_time = "life_time";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_machines_GetAll
                {
                    public const string name = "procedure_enp_machines_GetAll";
                }
                public struct procedure_enp_machines_CreateMachine
                {
                    public const string name = "procedure_enp_machines_CreateMachine";
                }
                public struct procedure_enp_machines_GetMachine
                {
                    public const string name = "procedure_enp_machines_GetMachine";
                }
                public struct procedure_enp_machines_UpdateMachine
                {
                    public const string name = "procedure_enp_machines_UpdateMachine";
                }
                public struct procedure_enp_machines_DeleteMachine
                {
                    public const string name = "procedure_enp_machines_DeleteMachine";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_machineItem>> GetAllAsync()
            {
                var machinesList = new List<enp_machineItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_machines_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            machinesList.Add(new enp_machineItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name]
                            });
                    }
                }
                return machinesList;
            }
            public static async Task<enp_machineItem> GetAsync(Int64 id)
            {
                var output = new enp_machineItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_machines_GetMachine.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.working_days_per_year = float.Parse(reader[columns.working_days_per_year].ToString());
                            output.working_hours_daily = float.Parse(reader[columns.working_hours_daily].ToString());
                            output.life_time = float.Parse(reader[columns.life_time].ToString());
                            output.price_per_uint = (Int64)reader[columns.price_per_uint];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_machineItem> CreateAsync(enp_machineItem machineItem)
            {
                var output = new enp_machineItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_machines_CreateMachine.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = machineItem.name;
                    cmd.Parameters.Add($"@{columns.working_days_per_year}", SqlDbType.Decimal).Value = machineItem.working_days_per_year;
                    cmd.Parameters.Add($"@{columns.working_hours_daily}", SqlDbType.Decimal).Value = machineItem.working_hours_daily;
                    cmd.Parameters.Add($"@{columns.life_time}", SqlDbType.Decimal).Value = machineItem.life_time;
                    cmd.Parameters.Add($"@{columns.price_per_uint}", SqlDbType.BigInt).Value = machineItem.price_per_uint;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)machineItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.working_days_per_year = float.Parse(reader[columns.working_days_per_year].ToString());
                            output.working_hours_daily = float.Parse(reader[columns.working_hours_daily].ToString());
                            output.life_time = float.Parse(reader[columns.life_time].ToString());
                            output.price_per_uint = (Int64)reader[columns.price_per_uint];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_machineItem> UpdateAsync(enp_machineItem machineItem)
            {
                var output = new enp_machineItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_machines_UpdateMachine.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = machineItem.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = machineItem.name;
                    cmd.Parameters.Add($"@{columns.working_days_per_year}", SqlDbType.Decimal).Value = machineItem.working_days_per_year;
                    cmd.Parameters.Add($"@{columns.working_hours_daily}", SqlDbType.Decimal).Value = machineItem.working_hours_daily;
                    cmd.Parameters.Add($"@{columns.life_time}", SqlDbType.Decimal).Value = machineItem.life_time;
                    cmd.Parameters.Add($"@{columns.price_per_uint}", SqlDbType.BigInt).Value = machineItem.price_per_uint;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)machineItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.working_days_per_year = float.Parse(reader[columns.working_days_per_year].ToString());
                            output.working_hours_daily = float.Parse(reader[columns.working_hours_daily].ToString());
                            output.life_time = float.Parse(reader[columns.life_time].ToString());
                            output.price_per_uint = (Int64)reader[columns.price_per_uint];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_machineItem machineItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_machines_DeleteMachine.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = machineItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_machines
        //{
        //    id,
        //    name,
        //    working_days_per_year,
        //    working_hours_daily,
        //    price_per_uint,
        //    life_time,
        //    description
        //}
        public struct enp_processes
        {
            public const string TableName = "enp_processes";
            public struct enp_processItem
            {
                public Int64 id;
                public string name;
                public enp_machines.enp_machineItem machine;
                public int operators_count;
                public float average_daily_production;
                public Int64 hourly_fee;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string machine_id = "machine_id";
                public const string operators_count = "operators_count";
                public const string average_daily_production = "average_daily_production";
                public const string hourly_fee = "hourly_fee";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_processes_GetAll
                {
                    public const string name = "procedure_enp_processes_GetAll";
                }
                public struct procedure_enp_processes_CreateProcess
                {
                    public const string name = "procedure_enp_processes_CreateProcess";
                }
                public struct procedure_enp_processes_GetProcess
                {
                    public const string name = "procedure_enp_processes_GetProcess";
                }
                public struct procedure_enp_processes_UpdateProcess
                {
                    public const string name = "procedure_enp_processes_UpdateProcess";
                }
                public struct procedure_enp_processes_DeleteProcess
                {
                    public const string name = "procedure_enp_processes_DeleteProcess";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_processItem>> GetAllAsync()
            {
                var machinesList = new List<enp_processItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_processes_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            machinesList.Add(new enp_processItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name]
                            });
                    }
                }
                return machinesList;
            }
            public static async Task<enp_processItem> GetAsync(Int64 id)
            {
                var output = new enp_processItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_processes_GetProcess.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.operators_count = (int)reader[columns.operators_count];
                            output.average_daily_production = float.Parse(reader[columns.average_daily_production].ToString());
                            output.hourly_fee = (Int64)reader[columns.hourly_fee];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                            output.machine = reader[columns.machine_id] != DBNull.Value ? new enp_machines.enp_machineItem() { id = (Int64)reader[columns.machine_id], name = (string)reader[enp_machines.GetFullColumnName(enp_machines.columns.name)] } : new enp_machines.enp_machineItem() { id = 0, name = "None" };
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_processItem> CreateAsync(enp_processItem processItem)
            {
                var output = new enp_processItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_processes_CreateProcess.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = processItem.name;
                    cmd.Parameters.Add($"@{columns.machine_id}", SqlDbType.BigInt).Value = processItem.machine.id > 0 ? (object)processItem.machine.id : DBNull.Value;
                    cmd.Parameters.Add($"@{columns.operators_count}", SqlDbType.Int).Value = processItem.operators_count;
                    cmd.Parameters.Add($"@{columns.average_daily_production}", SqlDbType.Decimal).Value = processItem.average_daily_production;
                    cmd.Parameters.Add($"@{columns.hourly_fee}", SqlDbType.Decimal).Value = processItem.hourly_fee;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)processItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.operators_count = Convert.ToInt16(reader[columns.operators_count]);
                            output.average_daily_production = float.Parse(reader[columns.average_daily_production].ToString());
                            output.hourly_fee = (Int64)reader[columns.hourly_fee];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                            output.machine = reader[columns.machine_id] != DBNull.Value ? new enp_machines.enp_machineItem() { id = (Int64)reader[columns.machine_id] } : new enp_machines.enp_machineItem() { id = 0 };
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_processItem> UpdateAsync(enp_processItem processItem)
            {
                var output = new enp_processItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_processes_UpdateProcess.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = processItem.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = processItem.name;
                    cmd.Parameters.Add($"@{columns.machine_id}", SqlDbType.BigInt).Value = processItem.machine.id > 0 ? (object)processItem.machine.id : DBNull.Value;
                    cmd.Parameters.Add($"@{columns.operators_count}", SqlDbType.Int).Value = processItem.operators_count;
                    cmd.Parameters.Add($"@{columns.average_daily_production}", SqlDbType.Decimal).Value = processItem.average_daily_production;
                    cmd.Parameters.Add($"@{columns.hourly_fee}", SqlDbType.Decimal).Value = processItem.hourly_fee;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)processItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.operators_count = Convert.ToInt16(reader[columns.operators_count]);
                            output.average_daily_production = float.Parse(reader[columns.average_daily_production].ToString());
                            output.hourly_fee = (Int64)reader[columns.hourly_fee];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                            output.machine = reader[columns.machine_id] != DBNull.Value ? new enp_machines.enp_machineItem() { id = (Int64)reader[columns.machine_id] } : new enp_machines.enp_machineItem() { id = 0 };
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_processItem processItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_processes_DeleteProcess.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = processItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_process
        //{
        //    id,
        //    name,
        //    machine_id,
        //    operators_count,
        //    average_daily_production,
        //    hourly_fee,
        //    description
        //}
        public struct enp_manufacturing_overhead
        {
            public const string TableName = "enp_manufacturing_overhead";
            public struct enp_manufacturing_overheadItem
            {
                public Int64 id;
                public string name;
                public Int64 yearly_fee;
                public Int32 yearly_production_capacity;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string yearly_fee = "yearly_fee";
                public const string yearly_production_capacity = "yearly_production_capacity";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_manufacturing_overhead_GetAll
                {
                    public const string name = "procedure_enp_manufacturing_overhead_GetAll";
                }
                public struct procedure_enp_manufacturing_overhead_CreateManufacturingOverhead
                {
                    public const string name = "procedure_enp_manufacturing_overhead_CreateManufacturingOverhead";
                }
                public struct procedure_enp_manufacturing_overhead_GetManufacturingOverhead
                {
                    public const string name = "procedure_enp_manufacturing_overhead_GetManufacturingOverhead";
                }
                public struct procedure_enp_manufacturing_overhead_UpdateManufacturingOverhead
                {
                    public const string name = "procedure_enp_manufacturing_overhead_UpdateManufacturingOverhead";
                }
                public struct procedure_enp_manufacturing_overhead_DeleteManufacturingOverhead
                {
                    public const string name = "procedure_enp_manufacturing_overhead_DeleteManufacturingOverhead";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_manufacturing_overheadItem>> GetAllAsync()
            {
                var machinesList = new List<enp_manufacturing_overheadItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_manufacturing_overhead_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            machinesList.Add(new enp_manufacturing_overheadItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name]
                            });
                    }
                }
                return machinesList;
            }
            public static async Task<enp_manufacturing_overheadItem> GetAsync(Int64 id)
            {
                var output = new enp_manufacturing_overheadItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_manufacturing_overhead_GetManufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.yearly_fee = (Int64)reader[columns.yearly_fee];
                            output.yearly_production_capacity = (Int32)reader[columns.yearly_production_capacity];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_manufacturing_overheadItem> CreateAsync(enp_manufacturing_overheadItem Item)
            {
                var output = new enp_manufacturing_overheadItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_manufacturing_overhead_CreateManufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = Item.name;
                    cmd.Parameters.Add($"@{columns.yearly_fee}", SqlDbType.BigInt).Value = Item.yearly_fee;
                    cmd.Parameters.Add($"@{columns.yearly_production_capacity}", SqlDbType.Int).Value = Item.yearly_production_capacity;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)Item.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.yearly_fee = (Int64)reader[columns.yearly_fee];
                            output.yearly_production_capacity = (Int32)reader[columns.yearly_production_capacity];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_manufacturing_overheadItem> UpdateAsync(enp_manufacturing_overheadItem Item)
            {
                var output = new enp_manufacturing_overheadItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_manufacturing_overhead_UpdateManufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = Item.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = Item.name;
                    cmd.Parameters.Add($"@{columns.yearly_fee}", SqlDbType.BigInt).Value = Item.yearly_fee;
                    cmd.Parameters.Add($"@{columns.yearly_production_capacity}", SqlDbType.Int).Value = Item.yearly_production_capacity;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)Item.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.yearly_fee = (Int64)reader[columns.yearly_fee];
                            output.yearly_production_capacity = (Int32)reader[columns.yearly_production_capacity];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_manufacturing_overheadItem Item)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_manufacturing_overhead_DeleteManufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = Item.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_manufacturing_overhead
        //{
        //    id,
        //    name,
        //    yearly_fee,
        //    share_percent,
        //    yearly_production_capacity,
        //    description
        //}
        public struct enp_molds
        {
            public const string TableName = "enp_molds";
            public struct enp_moldItem
            {
                public Int64 id;
                public string part_name;
                public string mold_id;
                public string tech_id;
                public string type;
                public Int32 life_time;
                public Int32 fastener_count;
                public Int64 design_fee;
                public Int64 material_fee;
                public Int64 machining_fee;
                public Int64 other_fee;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string part_name = "part_name";
                public const string mold_id = "mold_id";
                public const string tech_id = "tech_id";
                public const string type = "type";
                public const string life_time = "life_time";
                public const string fastener_count = "fastener_count";
                public const string design_fee = "design_fee";
                public const string material_fee = "material_fee";
                public const string machining_fee = "machining_fee";
                public const string other_fee = "other_fee";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_molds_GetAll
                {
                    public const string name = "procedure_enp_molds_GetAll";
                }
                public struct procedure_enp_molds_CreateMold
                {
                    public const string name = "procedure_enp_molds_CreateMold";
                }
                public struct procedure_enp_molds_GetMold
                {
                    public const string name = "procedure_enp_molds_GetMold";
                }
                public struct procedure_enp_molds_UpdateMold
                {
                    public const string name = "procedure_enp_molds_UpdateMold";
                }
                public struct procedure_enp_molds_DeleteMold
                {
                    public const string name = "procedure_enp_molds_DeleteMold";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_moldItem>> GetAllAsync()
            {
                var moldsList = new List<enp_moldItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_molds_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            moldsList.Add(new enp_moldItem()
                            {
                                id = (Int64)reader[columns.id],
                                part_name = (string)reader[columns.part_name],
                                mold_id = (string)reader[columns.mold_id],
                                tech_id = (string)reader[columns.tech_id]
                            });
                    }
                }
                return moldsList;
            }
            public static async Task<enp_moldItem> GetAsync(Int64 id)
            {
                var output = new enp_moldItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_molds_GetMold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.part_name = (string)reader[columns.part_name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.mold_id = (string)reader[columns.mold_id];
                            output.type = (string)reader[columns.type];
                            output.life_time = Convert.ToInt32(reader[columns.life_time].ToString());
                            output.fastener_count = Convert.ToInt32(reader[columns.fastener_count].ToString());
                            output.design_fee = Convert.ToInt64(reader[columns.design_fee].ToString());
                            output.material_fee = Convert.ToInt64(reader[columns.material_fee].ToString());
                            output.machining_fee = Convert.ToInt64(reader[columns.machining_fee].ToString());
                            output.other_fee = Convert.ToInt64(reader[columns.other_fee].ToString());
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_moldItem> CreateAsync(enp_moldItem moldItem)
            {
                var output = new enp_moldItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_molds_CreateMold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.part_name}", SqlDbType.NVarChar).Value = moldItem.part_name;
                    cmd.Parameters.Add($"@{columns.mold_id}", SqlDbType.NVarChar).Value = moldItem.mold_id;
                    cmd.Parameters.Add($"@{columns.tech_id}", SqlDbType.NVarChar).Value = moldItem.tech_id;
                    cmd.Parameters.Add($"@{columns.type}", SqlDbType.NVarChar).Value = moldItem.type;
                    cmd.Parameters.Add($"@{columns.life_time}", SqlDbType.Int).Value = moldItem.life_time;
                    cmd.Parameters.Add($"@{columns.fastener_count}", SqlDbType.Int).Value = moldItem.fastener_count;
                    cmd.Parameters.Add($"@{columns.design_fee}", SqlDbType.BigInt).Value = moldItem.design_fee;
                    cmd.Parameters.Add($"@{columns.material_fee}", SqlDbType.BigInt).Value = moldItem.material_fee;
                    cmd.Parameters.Add($"@{columns.machining_fee}", SqlDbType.BigInt).Value = moldItem.machining_fee;
                    cmd.Parameters.Add($"@{columns.other_fee}", SqlDbType.BigInt).Value = moldItem.other_fee;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)moldItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.part_name = (string)reader[columns.part_name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.mold_id = (string)reader[columns.mold_id];
                            output.type = (string)reader[columns.type];
                            output.life_time = Convert.ToInt32(reader[columns.life_time].ToString());
                            output.fastener_count = Convert.ToInt32(reader[columns.fastener_count].ToString());
                            output.design_fee = Convert.ToInt64(reader[columns.design_fee].ToString());
                            output.material_fee = Convert.ToInt64(reader[columns.material_fee].ToString());
                            output.machining_fee = Convert.ToInt64(reader[columns.machining_fee].ToString());
                            output.other_fee = Convert.ToInt64(reader[columns.other_fee].ToString());
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_moldItem> UpdateAsync(enp_moldItem moldItem)
            {
                var output = new enp_moldItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_molds_UpdateMold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = moldItem.id;
                    cmd.Parameters.Add($"@{columns.part_name}", SqlDbType.NVarChar).Value = moldItem.part_name;
                    cmd.Parameters.Add($"@{columns.mold_id}", SqlDbType.NVarChar).Value = moldItem.mold_id;
                    cmd.Parameters.Add($"@{columns.tech_id}", SqlDbType.NVarChar).Value = moldItem.tech_id;
                    cmd.Parameters.Add($"@{columns.type}", SqlDbType.NVarChar).Value = moldItem.type;
                    cmd.Parameters.Add($"@{columns.life_time}", SqlDbType.Int).Value = moldItem.life_time;
                    cmd.Parameters.Add($"@{columns.fastener_count}", SqlDbType.Int).Value = moldItem.fastener_count;
                    cmd.Parameters.Add($"@{columns.design_fee}", SqlDbType.BigInt).Value = moldItem.design_fee;
                    cmd.Parameters.Add($"@{columns.material_fee}", SqlDbType.BigInt).Value = moldItem.material_fee;
                    cmd.Parameters.Add($"@{columns.machining_fee}", SqlDbType.BigInt).Value = moldItem.machining_fee;
                    cmd.Parameters.Add($"@{columns.other_fee}", SqlDbType.BigInt).Value = moldItem.other_fee;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)moldItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.part_name = (string)reader[columns.part_name];
                            output.tech_id = (string)reader[columns.tech_id];
                            output.mold_id = (string)reader[columns.mold_id];
                            output.type = (string)reader[columns.type];
                            output.life_time = Convert.ToInt32(reader[columns.life_time].ToString());
                            output.fastener_count = Convert.ToInt32(reader[columns.fastener_count].ToString());
                            output.design_fee = Convert.ToInt64(reader[columns.design_fee].ToString());
                            output.material_fee = Convert.ToInt64(reader[columns.material_fee].ToString());
                            output.machining_fee = Convert.ToInt64(reader[columns.machining_fee].ToString());
                            output.other_fee = Convert.ToInt64(reader[columns.other_fee].ToString());
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_moldItem moldItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_molds_DeleteMold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = moldItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_molds
        //{
        //    id,
        //    mold_id,
        //    tech_id,
        //    type,
        //    fastener_count,
        //    design_fee,
        //    material_fee,
        //    machining_fee,
        //    other_fee,
        //    description
        //}
        public struct enp_packing
        {
            public const string TableName = "enp_packing";
            public struct enp_packingItem
            {
                public Int64 id;
                public string name;
                public Int64 price;
                public string description;
                public enp_units.enp_unit unit;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string unit_id = "unit_id";
                public const string price = "price";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_packing_GetAll
                {
                    public const string name = "procedure_enp_packing_GetAll";
                }
                public struct procedure_enp_packing_CreatePacking
                {
                    public const string name = "procedure_enp_packing_CreatePacking";
                }
                public struct procedure_enp_packing_GetPacking
                {
                    public const string name = "procedure_enp_packing_GetPacking";
                }
                public struct procedure_enp_packing_UpdatePacking
                {
                    public const string name = "procedure_enp_packing_UpdatePacking";
                }
                public struct procedure_enp_packing_DeletePacking
                {
                    public const string name = "procedure_enp_packing_DeletePacking";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_packingItem>> GetAllAsync()
            {
                var packingList = new List<enp_packingItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_packing_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            packingList.Add(new enp_packingItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name],
                                price = Convert.ToInt64(reader[columns.price])
                            });
                    }
                }
                return packingList;
            }
            public static async Task<enp_packingItem> CreateAsync(enp_packingItem packingItem)
            {
                var output = new enp_packingItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_packing_CreatePacking.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = packingItem.name;
                    cmd.Parameters.Add($"@{columns.unit_id}", SqlDbType.BigInt).Value = packingItem.unit.id;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = packingItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)packingItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.unit.id = (Int64)reader[columns.unit_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_packingItem> GetAsync(Int64 id)
            {
                var output = new enp_packingItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_packing_GetPacking.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                            output.unit = new enp_units.enp_unit() { id = (Int64)reader[columns.unit_id], name = (string)reader[enp_units.GetFullColumnName(enp_units.columns.name)] };
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_packingItem> UpdateAsync(enp_packingItem packingItem)
            {
                var output = new enp_packingItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_packing_UpdatePacking.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = packingItem.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = packingItem.name;
                    cmd.Parameters.Add($"@{columns.unit_id}", SqlDbType.BigInt).Value = packingItem.unit.id;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = packingItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)packingItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.unit.id = (Int64)reader[columns.unit_id];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_packingItem packingItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_packing_DeletePacking.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = packingItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_packing
        //{
        //    id,
        //    name,
        //    unit_id,
        //    price,
        //    description
        //}
        public struct enp_transportation
        {
            public const string TableName = "enp_transportation";
            public struct enp_transportationItem
            {
                public Int64 id;
                public string name;
                public Int64 price;
                public string description;
            }
            public struct columns
            {
                public const string id = "id";
                public const string name = "name";
                public const string price = "price";
                public const string description = "description";
            }
            public struct procedures
            {
                public struct procedure_enp_transportation_GetAll
                {
                    public const string name = "procedure_enp_transportation_GetAll";
                }
                public struct procedure_enp_transportation_CreateTransportation
                {
                    public const string name = "procedure_enp_transportation_CreateTransportation";
                }
                public struct procedure_enp_transportation_GetTransportation
                {
                    public const string name = "procedure_enp_transportation_GetTransportation";
                }
                public struct procedure_enp_transportation_UpdateTransportation
                {
                    public const string name = "procedure_enp_transportation_UpdateTransportation";
                }
                public struct procedure_enp_transportation_DeleteTransportation
                {
                    public const string name = "procedure_enp_transportation_DeleteTransportation";
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_transportationItem>> GetAllAsync()
            {
                var transportationList = new List<enp_transportationItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_transportation_GetAll.name, conn) { CommandType = CommandType.StoredProcedure }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                            transportationList.Add(new enp_transportationItem()
                            {
                                id = (Int64)reader[columns.id],
                                name = (string)reader[columns.name],
                                price = Convert.ToInt64(reader[columns.price])
                            });
                    }
                }
                return transportationList;
            }
            public static async Task<enp_transportationItem> CreateAsync(enp_transportationItem transportationItem)
            {
                var output = new enp_transportationItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_transportation_CreateTransportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = transportationItem.name;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = transportationItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)transportationItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_transportationItem> GetAsync(Int64 id)
            {
                var output = new enp_transportationItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_transportation_GetTransportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"{columns.id}", SqlDbType.BigInt).Value = id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_transportationItem> UpdateAsync(enp_transportationItem transportationItem)
            {
                var output = new enp_transportationItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_transportation_UpdateTransportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = transportationItem.id;
                    cmd.Parameters.Add($"@{columns.name}", SqlDbType.NVarChar).Value = transportationItem.name;
                    cmd.Parameters.Add($"@{columns.price}", SqlDbType.BigInt).Value = transportationItem.price;
                    cmd.Parameters.Add($"@{columns.description}", SqlDbType.NVarChar).Value = (object)transportationItem.description ?? DBNull.Value;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.name = (string)reader[columns.name];
                            output.price = (Int64)reader[columns.price];
                            output.description = reader[columns.description] != DBNull.Value ? (string)reader[columns.description] : "";
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_transportationItem transportationItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_transportation_DeleteTransportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = transportationItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        //public enum enp_transportation
        //{
        //    id,
        //    name,
        //    price,
        //    description
        //}
        public struct enp_bom
        {
            public const string TableName = "enp_bom";
            public struct enp_bomItem
            {
                public Int64 id;
                public enp_products.enp_product product;
                public class material_
                {
                    public enp_material.enp_materialItem item;
                    public decimal material_gross_consumption;
                    public decimal material_net_consumption;
                    public string material_part_name;
                }
                public material_ material;
                public class part_
                {
                    public enp_parts.enp_partItem item;
                    public decimal part_consumption_coefficient;
                }
                public part_ part;
                public class process_
                {
                    public enp_processes.enp_processItem item;
                    public decimal process_consumption_coefficient;
                }
                public process_ process;
                public class mold_
                {
                    public enp_molds.enp_moldItem item;
                    public decimal mold_consumption_coefficient;
                }
                public mold_ mold;
                public class manufacturing_overhead_
                {
                    public enp_manufacturing_overhead.enp_manufacturing_overheadItem item;
                    public decimal manufacturing_overhead_share_percent;
                }
                public manufacturing_overhead_ manufacturing_overhead;
                public class packing_
                { 
                    public enp_packing.enp_packingItem item;
                    public decimal packing_consumption_coefficient;
                }
                public packing_ packing;
                public class transportation_
                {
                    public enp_transportation.enp_transportationItem item;
                    public decimal transportation_consumption_coefficient;
                }
                public transportation_ transportation;
            }
            public struct columns
            {
                public const string id = "id";
                public const string product_id = "product_id";
                public const string material_id = "material_id";
                public const string material_gross_consumption = "material_gross_consumption";
                public const string material_net_consumption = "material_net_consumption";
                public const string material_part_name = "material_part_name";
                public const string part_id = "part_id";
                public const string part_consumption_coefficient = "part_consumption_coefficient";
                public const string process_id = "process_id";
                public const string process_consumption_coefficient = "process_consumption_coefficient";
                public const string mold_id = "mold_id";
                public const string mold_consumption_coefficient = "mold_consumption_coefficient";
                public const string manufacturing_overhead_id = "manufacturing_overhead_id";
                public const string manufacturing_overhead_share_percent = "manufacturing_overhead_share_percent";
                public const string packing_id = "packing_id";
                public const string packing_consumption_coefficient = "packing_consumption_coefficient";
                public const string transportation_id = "transportation_id";
                public const string transportation_consumption_coefficient = "transportation_consumption_coefficient";
            }
            public struct procedures
            {
                public struct procedure_enp_bom_GetAll
                {
                    public const string name = "procedure_enp_bom_GetAll";
                }
                public struct procedure_enp_bom_CreateMaterial
                {
                    public const string name = "procedure_enp_bom_CreateMaterial";
                }
                public struct procedure_enp_bom_UpdateMaterial
                {
                    public const string name = "procedure_enp_bom_UpdateMaterial";
                }
                public struct procedure_enp_bom_CreatePart
                {
                    public const string name = "procedure_enp_bom_CreatePart";
                }
                public struct procedure_enp_bom_UpdatePart
                {
                    public const string name = "procedure_enp_bom_UpdatePart";
                }
                public struct procedure_enp_bom_CreateProcess
                {
                    public const string name = "procedure_enp_bom_CreateProcess";
                }
                public struct procedure_enp_bom_UpdateProcess
                {
                    public const string name = "procedure_enp_bom_UpdateProcess";
                }
                public struct procedure_enp_bom_CreateMold
                {
                    public const string name = "procedure_enp_bom_CreateMold";
                }
                public struct procedure_enp_bom_UpdateMold
                {
                    public const string name = "procedure_enp_bom_UpdateMold";
                }
                public struct procedure_enp_bom_CreateManufacturingOverhead
                {
                    public const string name = "procedure_enp_bom_CreateManufacturingOverhead";
                }
                public struct procedure_enp_bom_UpdateManufacturingOverhead
                {
                    public const string name = "procedure_enp_bom_UpdateManufacturingOverhead";
                }
                public struct procedure_enp_bom_CreatePacking
                {
                    public const string name = "procedure_enp_bom_CreatePacking";
                }
                public struct procedure_enp_bom_UpdatePacking
                {
                    public const string name = "procedure_enp_bom_UpdatePacking";
                }
                public struct procedure_enp_bom_CreateTransportation
                {
                    public const string name = "procedure_enp_bom_CreateTransportation";
                }
                public struct procedure_enp_bom_UpdateTransportation
                {
                    public const string name = "procedure_enp_bom_UpdateTransportation";
                }
                public struct procedure_enp_bom_DeleteBOM
                {
                    public const string name = "procedure_enp_bom_DeleteBOM";
                }
                public struct reports
                {
                    public struct procedure_enp_bom_report_material
                    {
                        public const string name = "procedure_enp_bom_report_material";
                    }
                    public struct procedure_enp_bom_report_part
                    {
                        public const string name = "procedure_enp_bom_report_part";
                    }
                    public struct procedure_enp_bom_report_process
                    {
                        public const string name = "procedure_enp_bom_report_process";
                    }
                    public struct procedure_enp_bom_report_manufacturingOverhead
                    {
                        public const string name = "procedure_enp_bom_report_manufacturingOverhead";
                    }
                    public struct procedure_enp_bom_report_mold
                    {
                        public const string name = "procedure_enp_bom_report_mold";
                    }
                    public struct procedure_enp_bom_report_packing
                    {
                        public const string name = "procedure_enp_bom_report_packing";
                    }
                    public struct procedure_enp_bom_report_transportation
                    {
                        public const string name = "procedure_enp_bom_report_transportation";
                    }
                }
            }
            public static string GetFullColumnName(string column)
            {
                return $"{TableName}_{column}";
            }
            public static async Task<List<enp_bomItem>> GetAllAsync(enp_products.enp_product product)
            {
                var bomList = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var reader = await new SqlCommand(procedures.procedure_enp_bom_GetAll.name, conn) { CommandType = CommandType.StoredProcedure, Parameters = { new SqlParameter($"@{columns.product_id}", SqlDbType.BigInt) { Value = product.id } } }.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var bom = new enp_bomItem() { id = Convert.ToInt64(reader[columns.id]), product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) } };
                            if (reader[columns.material_id] != DBNull.Value )
                            {
                                bom.material = new enp_bomItem.material_();
                                bom.material.item = new enp_material.enp_materialItem() { id = Convert.ToInt64(reader[columns.material_id]), name = (string)reader["name"] };
                                bom.material.material_net_consumption = Convert.ToDecimal(reader[columns.material_net_consumption]);
                                bom.material.material_gross_consumption = Convert.ToDecimal(reader[columns.material_gross_consumption]);
                                bom.material.material_part_name = (string)reader[columns.material_part_name];
                            }
                            else if (reader[columns.part_id] != DBNull.Value)
                            {
                                bom.part = new enp_bomItem.part_();
                                bom.part.item = new enp_parts.enp_partItem() { id = Convert.ToInt64(reader[columns.part_id]), name = (string)reader["name"] };
                                bom.part.part_consumption_coefficient = Convert.ToDecimal(reader[columns.part_consumption_coefficient]);
                            }
                            else if (reader[columns.process_id] != DBNull.Value)
                            {
                                bom.process = new enp_bomItem.process_();
                                bom.process.item = new enp_processes.enp_processItem() { id = Convert.ToInt64(reader[columns.process_id]), name = (string)reader["name"] };
                                bom.process.process_consumption_coefficient = Convert.ToDecimal(reader[columns.process_consumption_coefficient]);
                            }
                            else if (reader[columns.mold_id] != DBNull.Value)
                            {
                                bom.mold = new enp_bomItem.mold_();
                                bom.mold.item = new enp_molds.enp_moldItem() { id = Convert.ToInt64(reader[columns.mold_id]), part_name = (string)reader["name"] };
                                bom.mold.mold_consumption_coefficient = Convert.ToDecimal(reader[columns.mold_consumption_coefficient]);
                            }
                            else if (reader[columns.manufacturing_overhead_id] != DBNull.Value)
                            {
                                bom.manufacturing_overhead = new enp_bomItem.manufacturing_overhead_();
                                bom.manufacturing_overhead.item = new enp_manufacturing_overhead.enp_manufacturing_overheadItem() { id = Convert.ToInt64(reader[columns.manufacturing_overhead_id]), name = (string)reader["name"] };
                                bom.manufacturing_overhead.manufacturing_overhead_share_percent = Convert.ToDecimal(reader[columns.manufacturing_overhead_share_percent]);
                            }
                            else if (reader[columns.packing_id] != DBNull.Value)
                            {
                                bom.packing = new enp_bomItem.packing_();
                                bom.packing.item = new enp_packing.enp_packingItem() { id = Convert.ToInt64(reader[columns.packing_id]), name = (string)reader["name"] };
                                bom.packing.packing_consumption_coefficient = Convert.ToDecimal(reader[columns.packing_consumption_coefficient]);
                            }
                            else if (reader[columns.transportation_id] != DBNull.Value)
                            {
                                bom.transportation = new enp_bomItem.transportation_();
                                bom.transportation.item = new enp_transportation.enp_transportationItem() { id = Convert.ToInt64(reader[columns.transportation_id]), name = (string)reader["name"] };
                                bom.transportation.transportation_consumption_coefficient = Convert.ToDecimal(reader[columns.transportation_consumption_coefficient]);
                            }
                            bomList.Add(bom);
                        }
                    }
                }
                return bomList;
            }
            public static async Task<enp_bomItem> CreateMaterialAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreateMaterial.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.material_id}", SqlDbType.BigInt).Value = bomItem.material.item.id;
                    cmd.Parameters.Add($"@{columns.material_net_consumption}", SqlDbType.Decimal).Value = bomItem.material.material_net_consumption;
                    cmd.Parameters.Add($"@{columns.material_gross_consumption}", SqlDbType.Decimal).Value = bomItem.material.material_gross_consumption;
                    cmd.Parameters.Add($"@{columns.material_part_name}", SqlDbType.NVarChar).Value = bomItem.material.material_part_name;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.material = new enp_bomItem.material_();
                            output.material.item = new enp_material.enp_materialItem() { id = Convert.ToInt64(reader[columns.material_id]) };
                            output.material.material_net_consumption = Convert.ToDecimal(reader[columns.material_net_consumption]);
                            output.material.material_gross_consumption = Convert.ToDecimal(reader[columns.material_gross_consumption]);
                            output.material.material_part_name = (string)reader[columns.material_part_name];
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdateMaterialAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdateMaterial.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.material_id}", SqlDbType.BigInt).Value = bomItem.material.item.id;
                    cmd.Parameters.Add($"@{columns.material_net_consumption}", SqlDbType.Decimal).Value = bomItem.material.material_net_consumption;
                    cmd.Parameters.Add($"@{columns.material_gross_consumption}", SqlDbType.Decimal).Value = bomItem.material.material_gross_consumption;
                    cmd.Parameters.Add($"@{columns.material_part_name}", SqlDbType.NVarChar).Value = bomItem.material.material_part_name;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.material = new enp_bomItem.material_();
                            output.material.item = new enp_material.enp_materialItem() { id = Convert.ToInt64(reader[columns.material_id]) };
                            output.material.material_net_consumption = Convert.ToDecimal(reader[columns.material_net_consumption]);
                            output.material.material_gross_consumption = Convert.ToDecimal(reader[columns.material_gross_consumption]);
                            output.material.material_part_name = (string)reader[columns.material_part_name];
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> CreatePartAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreatePart.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.part_id}", SqlDbType.BigInt).Value = bomItem.part.item.id;
                    cmd.Parameters.Add($"@{columns.part_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.part.part_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.part = new enp_bomItem.part_();
                            output.part.item = new enp_parts.enp_partItem() { id = Convert.ToInt64(reader[columns.part_id]) };
                            output.part.part_consumption_coefficient = Convert.ToDecimal(reader[columns.part_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdatePartAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdatePart.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.part_id}", SqlDbType.BigInt).Value = bomItem.part.item.id;
                    cmd.Parameters.Add($"@{columns.part_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.part.part_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.part = new enp_bomItem.part_();
                            output.part.item = new enp_parts.enp_partItem() { id = Convert.ToInt64(reader[columns.part_id]) };
                            output.part.part_consumption_coefficient = Convert.ToDecimal(reader[columns.part_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> CreateProcessAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreateProcess.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.process_id}", SqlDbType.BigInt).Value = bomItem.process.item.id;
                    cmd.Parameters.Add($"@{columns.process_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.process.process_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.process = new enp_bomItem.process_();
                            output.process.item = new enp_processes.enp_processItem() { id = Convert.ToInt64(reader[columns.process_id]) };
                            output.process.process_consumption_coefficient = Convert.ToDecimal(reader[columns.process_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdateProcessAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdateProcess.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.process_id}", SqlDbType.BigInt).Value = bomItem.process.item.id;
                    cmd.Parameters.Add($"@{columns.process_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.process.process_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.process = new enp_bomItem.process_();
                            output.process.item = new enp_processes.enp_processItem() { id = Convert.ToInt64(reader[columns.process_id]) };
                            output.process.process_consumption_coefficient = Convert.ToDecimal(reader[columns.process_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> CreateMoldAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreateMold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.mold_id}", SqlDbType.BigInt).Value = bomItem.mold.item.id;
                    cmd.Parameters.Add($"@{columns.mold_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.mold.mold_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.mold = new enp_bomItem.mold_();
                            output.mold.item = new enp_molds.enp_moldItem() { id = Convert.ToInt64(reader[columns.mold_id]) };
                            output.mold.mold_consumption_coefficient = Convert.ToDecimal(reader[columns.mold_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdateMoldAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdateMold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.mold_id}", SqlDbType.BigInt).Value = bomItem.mold.item.id;
                    cmd.Parameters.Add($"@{columns.mold_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.mold.mold_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.mold = new enp_bomItem.mold_();
                            output.mold.item = new enp_molds.enp_moldItem() { id = Convert.ToInt64(reader[columns.mold_id]) };
                            output.mold.mold_consumption_coefficient = Convert.ToDecimal(reader[columns.mold_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> CreateManufacturingOverheadAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreateManufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.manufacturing_overhead_id}", SqlDbType.BigInt).Value = bomItem.manufacturing_overhead.item.id;
                    cmd.Parameters.Add($"@{columns.manufacturing_overhead_share_percent}", SqlDbType.Decimal).Value = bomItem.manufacturing_overhead.manufacturing_overhead_share_percent;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.manufacturing_overhead = new enp_bomItem.manufacturing_overhead_();
                            output.manufacturing_overhead.item = new enp_manufacturing_overhead.enp_manufacturing_overheadItem() { id = Convert.ToInt64(reader[columns.manufacturing_overhead_id]) };
                            output.manufacturing_overhead.manufacturing_overhead_share_percent = Convert.ToDecimal(reader[columns.manufacturing_overhead_share_percent]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdateManufacturingOverheadAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdateManufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.manufacturing_overhead_id}", SqlDbType.BigInt).Value = bomItem.manufacturing_overhead.item.id;
                    cmd.Parameters.Add($"@{columns.manufacturing_overhead_share_percent}", SqlDbType.Decimal).Value = bomItem.manufacturing_overhead.manufacturing_overhead_share_percent;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.manufacturing_overhead = new enp_bomItem.manufacturing_overhead_();
                            output.manufacturing_overhead.item = new enp_manufacturing_overhead.enp_manufacturing_overheadItem() { id = Convert.ToInt64(reader[columns.manufacturing_overhead_id]) };
                            output.manufacturing_overhead.manufacturing_overhead_share_percent = Convert.ToDecimal(reader[columns.manufacturing_overhead_share_percent]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> CreatePackingAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreatePacking.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.packing_id}", SqlDbType.BigInt).Value = bomItem.packing.item.id;
                    cmd.Parameters.Add($"@{columns.packing_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.packing.packing_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.packing = new enp_bomItem.packing_();
                            output.packing.item = new enp_packing.enp_packingItem() { id = Convert.ToInt64(reader[columns.packing_id]) };
                            output.packing.packing_consumption_coefficient = Convert.ToDecimal(reader[columns.packing_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdatePackingAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdatePacking.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.packing_id}", SqlDbType.BigInt).Value = bomItem.packing.item.id;
                    cmd.Parameters.Add($"@{columns.packing_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.packing.packing_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.packing = new enp_bomItem.packing_();
                            output.packing.item = new enp_packing.enp_packingItem() { id = Convert.ToInt64(reader[columns.packing_id]) };
                            output.packing.packing_consumption_coefficient = Convert.ToDecimal(reader[columns.packing_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> CreateTransportationAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_CreateTransportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = bomItem.product.id;
                    cmd.Parameters.Add($"@{columns.transportation_id}", SqlDbType.BigInt).Value = bomItem.transportation.item.id;
                    cmd.Parameters.Add($"@{columns.transportation_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.transportation.transportation_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.transportation = new enp_bomItem.transportation_();
                            output.transportation.item = new enp_transportation.enp_transportationItem() { id = Convert.ToInt64(reader[columns.transportation_id]) };
                            output.transportation.transportation_consumption_coefficient = Convert.ToDecimal(reader[columns.transportation_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task<enp_bomItem> UpdateTransportationAsync(enp_bomItem bomItem)
            {
                var output = new enp_bomItem();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_UpdateTransportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    cmd.Parameters.Add($"@{columns.transportation_id}", SqlDbType.BigInt).Value = bomItem.transportation.item.id;
                    cmd.Parameters.Add($"@{columns.transportation_consumption_coefficient}", SqlDbType.Decimal).Value = bomItem.transportation.transportation_consumption_coefficient;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.id = (Int64)reader[columns.id];
                            output.product = new enp_products.enp_product() { id = Convert.ToInt64(reader[columns.product_id]) };
                            output.transportation = new enp_bomItem.transportation_();
                            output.transportation.item = new enp_transportation.enp_transportationItem() { id = Convert.ToInt64(reader[columns.transportation_id]) };
                            output.transportation.transportation_consumption_coefficient = Convert.ToDecimal(reader[columns.transportation_consumption_coefficient]);
                        }
                    }
                }
                return output;
            }
            public static async Task DeleteAsync(enp_bomItem bomItem)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.procedure_enp_bom_DeleteBOM.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.id}", SqlDbType.BigInt).Value = bomItem.id;
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            public static async Task<List<enp_bomItem>> GetMaterialReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_material.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.Add(new enp_bomItem()
                            {
                                material = new enp_bomItem.material_()
                                {
                                    material_gross_consumption = Convert.ToDecimal(reader[columns.material_gross_consumption]),
                                    material_net_consumption = Convert.ToDecimal(reader[columns.material_net_consumption]),
                                    material_part_name = (string)reader[columns.material_part_name],
                                    item = new enp_material.enp_materialItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_material.GetFullColumnName(enp_material.columns.id)]),
                                        name = (string)reader[enp_material.GetFullColumnName(enp_material.columns.name)],
                                        tech_id = (string)reader[enp_material.GetFullColumnName(enp_material.columns.tech_id)],
                                        price = Convert.ToInt64(reader[enp_material.GetFullColumnName(enp_material.columns.price)]),
                                        unit = new enp_units.enp_unit()
                                        {
                                            name = (string)reader[enp_units.GetFullColumnName(enp_units.columns.name)]
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                return output;
            }
            public static async Task<List<enp_bomItem>> GetPartReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_part.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.Add(new enp_bomItem()
                            {
                                part = new enp_bomItem.part_()
                                {
                                    part_consumption_coefficient = Convert.ToDecimal(reader[columns.part_consumption_coefficient]),
                                    item = new enp_parts.enp_partItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_parts.GetFullColumnName(enp_parts.columns.id)]),
                                        name = (string)reader[enp_parts.GetFullColumnName(enp_parts.columns.name)],
                                        tech_id = (string)reader[enp_parts.GetFullColumnName(enp_parts.columns.tech_id)],
                                        price = Convert.ToInt64(reader[enp_parts.GetFullColumnName(enp_parts.columns.price)]),
                                        unit = new enp_units.enp_unit()
                                        {
                                            name = (string)reader[enp_units.GetFullColumnName(enp_units.columns.name)]
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                return output;
            }
            public static async Task<List<enp_bomItem>> GetProcessReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_process.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new enp_bomItem()
                            {
                                process = new enp_bomItem.process_()
                                {
                                    process_consumption_coefficient = Convert.ToDecimal(reader[columns.process_consumption_coefficient]),
                                    item = new enp_processes.enp_processItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_processes.GetFullColumnName(enp_processes.columns.id)]),
                                        name = (string)reader[enp_processes.GetFullColumnName(enp_processes.columns.name)],
                                        operators_count = Convert.ToInt16(reader[enp_processes.GetFullColumnName(enp_processes.columns.operators_count)]),
                                        average_daily_production = float.Parse(reader[enp_processes.GetFullColumnName(enp_processes.columns.average_daily_production)].ToString()),
                                        hourly_fee = Convert.ToInt64(reader[enp_processes.GetFullColumnName(enp_processes.columns.hourly_fee)])
                                    }
                                }
                            };
                            if (reader[enp_machines.GetFullColumnName(enp_machines.columns.id)] != DBNull.Value)
                            {
                                item.process.item.machine = new enp_machines.enp_machineItem()
                                {
                                    id = Convert.ToInt64(reader[enp_machines.GetFullColumnName(enp_machines.columns.id)]),
                                    name = (string)reader[enp_machines.GetFullColumnName(enp_machines.columns.name)],
                                    working_days_per_year = float.Parse(reader[enp_machines.GetFullColumnName(enp_machines.columns.working_days_per_year)].ToString()),
                                    working_hours_daily = float.Parse(reader[enp_machines.GetFullColumnName(enp_machines.columns.working_hours_daily)].ToString()),
                                    price_per_uint = Convert.ToInt64(reader[enp_machines.GetFullColumnName(enp_machines.columns.price_per_uint)]),
                                    life_time = float.Parse(reader[enp_machines.GetFullColumnName(enp_machines.columns.life_time)].ToString())
                                };
                            }
                            else item.process.item.machine = new enp_machines.enp_machineItem() { id = 0 };
                            output.Add(item);
                        }
                    }
                }
                return output;
            }
            public static async Task<List<enp_bomItem>> GetMoldReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_mold.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.Add(new enp_bomItem()
                            {
                                mold = new enp_bomItem.mold_()
                                {
                                    mold_consumption_coefficient = Convert.ToDecimal(reader[columns.mold_consumption_coefficient]),
                                    item = new enp_molds.enp_moldItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_molds.GetFullColumnName(enp_molds.columns.id)]),
                                        part_name = (string)reader[enp_molds.GetFullColumnName(enp_molds.columns.part_name)],
                                        mold_id = (string)reader[enp_molds.GetFullColumnName(enp_molds.columns.mold_id)],
                                        tech_id = (string)reader[enp_molds.GetFullColumnName(enp_molds.columns.tech_id)],
                                        type = (string)reader[enp_molds.GetFullColumnName(enp_molds.columns.type)],
                                        life_time = Convert.ToInt32(reader[enp_molds.GetFullColumnName(enp_molds.columns.life_time)]),
                                        fastener_count = Convert.ToInt32(reader[enp_molds.GetFullColumnName(enp_molds.columns.fastener_count)]),
                                        design_fee = Convert.ToInt64(reader[enp_molds.GetFullColumnName(enp_molds.columns.design_fee)]),
                                        material_fee = Convert.ToInt64(reader[enp_molds.GetFullColumnName(enp_molds.columns.material_fee)]),
                                        machining_fee = Convert.ToInt64(reader[enp_molds.GetFullColumnName(enp_molds.columns.machining_fee)]),
                                        other_fee = Convert.ToInt64(reader[enp_molds.GetFullColumnName(enp_molds.columns.other_fee)])
                                    }
                                }
                            });
                        }
                    }
                }
                return output;
            }
            public static async Task<List<enp_bomItem>> GetManufacturingOverheadReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_manufacturingOverhead.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.Add(new enp_bomItem()
                            {
                                manufacturing_overhead = new enp_bomItem.manufacturing_overhead_()
                                {
                                    manufacturing_overhead_share_percent = Convert.ToDecimal(reader[columns.manufacturing_overhead_share_percent]),
                                    item = new enp_manufacturing_overhead.enp_manufacturing_overheadItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.id)]),
                                        name = (string)reader[enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.name)],
                                        yearly_fee = Convert.ToInt64(reader[enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.yearly_fee)]),
                                        yearly_production_capacity = Convert.ToInt32(reader[enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.yearly_production_capacity)]),
                                    }
                                }
                            });
                        }
                    }
                }
                return output;
            }
            public static async Task<List<enp_bomItem>> GetPckingReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_packing.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.Add(new enp_bomItem()
                            {
                                packing = new enp_bomItem.packing_()
                                {
                                    packing_consumption_coefficient = Convert.ToDecimal(reader[columns.packing_consumption_coefficient]),
                                    item = new enp_packing.enp_packingItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_packing.GetFullColumnName(enp_packing.columns.id)]),
                                        name = (string)reader[enp_packing.GetFullColumnName(enp_packing.columns.name)],
                                        price = Convert.ToInt64(reader[enp_packing.GetFullColumnName(enp_packing.columns.price)]),
                                        unit = new enp_units.enp_unit()
                                        {
                                            name = (string)reader[enp_units.GetFullColumnName(enp_units.columns.name)]
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                return output;
            }
            public static async Task<List<enp_bomItem>> GetTransportationReport(enp_products.enp_product product)
            {
                var output = new List<enp_bomItem>();
                using (var conn = new SqlConnection(ConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand(procedures.reports.procedure_enp_bom_report_transportation.name, conn) { CommandType = CommandType.StoredProcedure };
                    cmd.Parameters.Add($"@{columns.product_id}", SqlDbType.BigInt).Value = product.id;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            output.Add(new enp_bomItem()
                            {
                                transportation = new enp_bomItem.transportation_()
                                {
                                    transportation_consumption_coefficient = Convert.ToDecimal(reader[columns.transportation_consumption_coefficient]),
                                    item = new enp_transportation.enp_transportationItem()
                                    {
                                        id = Convert.ToInt64(reader[enp_transportation.GetFullColumnName(enp_transportation.columns.id)]),
                                        name = (string)reader[enp_transportation.GetFullColumnName(enp_transportation.columns.name)],
                                        price = Convert.ToInt64(reader[enp_transportation.GetFullColumnName(enp_transportation.columns.price)])
                                    }
                                }
                            });
                        }
                    }
                }
                return output;
            }
        }
        //public enum enp_bom
        //{
        //    id,
        //    product_id,
        //    material_id,
        //    material_gross_consumption,
        //    material_net_consumption,
        //    part_id,
        //    part_consumption_coefficient,
        //    process_id,
        //    process_consumption_coefficient,
        //    mold_id,
        //    mold_consumption_coefficient,
        //    manufacturing_overhead_id,
        //    manufacturing_overhead_consumption_coefficient,
        //    packing_id,
        //    packing_consumption_coefficient,
        //    transportation_id,
        //    transportation_consumption_coefficient
        //}
        public Dictionary<object, string> enp_EN_dict = new Dictionary<object, string>()
        {
            { enp_Table.enp_products, "Products" },
            { enp_Table.enp_product_id_types, "Products' ID Type" },
            { enp_Table.enp_units, "Units" },
            { enp_Table.enp_material, "Material" },
            { enp_Table.enp_parts, "Parts" },
            { enp_Table.enp_machines, "Machines" },
            { enp_Table.enp_processes, "Processes" },
            { enp_Table.enp_manufacturing_overhead, "Manufacturing Overhead" },
            { enp_Table.enp_molds, "Molds" }
        };
        public static async Task CreateDatabase()
        {
            var sequences = new Dictionary<string, string>();
            {
                //Create Sequence
                var sequenceName = "enp_global_unique_id";
                sequences.Add(sequenceName,
                    $@"CREATE SEQUENCE {sequenceName} 
                    AS BIGINT
                    START WITH 1
                    INCREMENT BY 1;"
                    );
                //Create Tables
                var tables = new Dictionary<string, string>();
                //Products Table
                {
                    var tableName = enp_products.TableName;
                    tables.Add(tableName,
                        $@"CREATE TABLE {tableName} (
                        {enp_products.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_products.columns.name} NVARCHAR(1024) UNIQUE,
                        {enp_products.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );

                        GO

                        CREATE TRIGGER tr_{tableName} 
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_products.columns.id}, {enp_products.columns.name}, {enp_products.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_products.columns.name}, {enp_products.columns.description}
                            FROM inserted
                        END;");
                }
                //Products Id Types Table
                {
                    var tableName = enp_product_id_types.TableName;
                    tables.Add(tableName,
                        $@"CREATE TABLE {tableName} (
                        {enp_product_id_types.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_product_id_types.columns.name} NVARCHAR(256) NOT NULL UNIQUE,
                        {enp_product_id_types.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );

                        GO

                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_product_id_types.columns.id}, {enp_product_id_types.columns.name}, {enp_product_id_types.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_product_id_types.columns.name}, {enp_product_id_types.columns.description}
                            FROM inserted
                        END;");
                }
                //Product Ids
                {
                    var tableName = enp_product_ids.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_product_ids.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_product_ids.columns.product_id} BIGINT NOT NULL,
                        {enp_product_ids.columns.product_id_type_id} BIGINT NOT NULL,
                        {enp_product_ids.columns.value} NVARCHAR(256) NOT NULL,
                        CONSTRAINT fk_{tableName}_{enp_product_ids.columns.product_id}
                        FOREIGN KEY ({enp_product_ids.columns.product_id})
                        REFERENCES {enp_Table.enp_products}({enp_products.columns.id})
                        ON DELETE NO ACTION
                        ON UPDATE NO ACTION,
                        CONSTRAINT fk_{tableName}_{enp_product_ids.columns.product_id_type_id}
                        FOREIGN KEY ({enp_product_ids.columns.product_id_type_id})
                        REFERENCES {enp_Table.enp_product_id_types}({enp_product_id_types.columns.id})
                        ON DELETE NO ACTION
                        ON UPDATE NO ACTION
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_product_ids.columns.id}, {enp_product_ids.columns.product_id}, {enp_product_ids.columns.product_id_type_id}, {enp_product_ids.columns.value})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_product_ids.columns.product_id}, {enp_product_ids.columns.product_id_type_id}, {enp_product_ids.columns.value}
                            FROM inserted
                        END;
                        CREATE INDEX IX_{tableName}_{enp_product_ids.columns.product_id_type_id} ON {tableName}({enp_product_ids.columns.product_id_type_id});");
                }
                //Units Table
                {
                    var tableName = enp_units.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_units.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_units.columns.name} NVARCHAR(256) UNIQUE NOT NULL,
                        {enp_units.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_units.columns.id}, {enp_units.columns.name}, {enp_units.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_units.columns.name}, {enp_units.columns.description}
                            FROM inserted
                        END;");
                }
                //Material Table
                {
                    var tableName = enp_material.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_material.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_material.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_material.columns.tech_id} NVARCHAR(128) NOT NULL UNIQUE,
                        {enp_material.columns.unit_id} BIGINT NOT NULL,
                        {enp_material.columns.price} BIGINT NOT NULL DEFAULT 0,
                        {enp_material.columns.description} NVARCHAR(MAX) DEFAULT N'',
                        CONSTRAINT fk_{tableName}_unit
                        FOREIGN KEY ({enp_material.columns.unit_id})
                        REFERENCES {enp_Table.enp_units}({enp_units.columns.id})
                        ON DELETE NO ACTION
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_material.columns.id}, {enp_material.columns.name}, {enp_material.columns.tech_id}, {enp_material.columns.unit_id}, {enp_material.columns.price}, {enp_material.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_material.columns.name}, {enp_material.columns.tech_id}, {enp_material.columns.unit_id}, {enp_material.columns.price}, {enp_material.columns.description}
                            FROM inserted
                        END;");
                }
                //Parts Table
                {
                    var tableName = enp_parts.TableName;
                    tables.Add(tableName,
                        $@"CREATE TABLE {tableName} (
                        {enp_parts.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_parts.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_parts.columns.tech_id} NVARCHAR(128) NOT NULL,
                        {enp_parts.columns.unit_id} BIGINT NOT NULL,
                        {enp_parts.columns.price} BIGINT NOT NULL DEFAULT 0,
                        {enp_parts.columns.description} NVARCHAR(MAX) DEFAULT N'',
                        CONSTRAINT fk_{tableName}_unit
                        FOREIGN KEY ({enp_parts.columns.unit_id})
                        REFERENCES {enp_Table.enp_units}({enp_units.columns.id})
                        ON DELETE NO ACTION
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_parts.columns.id}, {enp_parts.columns.name}, {enp_parts.columns.tech_id}, {enp_parts.columns.unit_id}, {enp_parts.columns.price}, {enp_parts.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_parts.columns.name}, {enp_parts.columns.tech_id}, {enp_parts.columns.unit_id}, {enp_parts.columns.price}, {enp_parts.columns.description}
                            FROM inserted
                        END;");
                }
                //Machines Table
                {
                    var tableName = enp_machines.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_machines.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_machines.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_machines.columns.working_days_per_year} DECIMAL(4, 1) NOT NULL,
                        {enp_machines.columns.working_hours_daily} DECIMAL(3, 1) NOT NULL,
                        {enp_machines.columns.price_per_uint} BIGINT NOT NULL,
                        {enp_machines.columns.life_time} DECIMAL(6, 3) NOT NULL,
                        {enp_machines.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_machines.columns.id}, {enp_machines.columns.name}, {enp_machines.columns.working_days_per_year}, {enp_machines.columns.working_hours_daily}, {enp_machines.columns.price_per_uint}, {enp_machines.columns.life_time}, {enp_machines.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_machines.columns.name}, {enp_machines.columns.working_days_per_year}, {enp_machines.columns.working_hours_daily}, {enp_machines.columns.price_per_uint}, {enp_machines.columns.life_time}, {enp_machines.columns.description}
                            FROM inserted
                        END;");
                }
                //Process Table
                {
                    var tableName = enp_processes.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_processes.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_processes.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_processes.columns.machine_id} BIGINT NULL,
                        {enp_processes.columns.operators_count} INT NOT NULL DEFAULT 0,
                        {enp_processes.columns.average_daily_production} DECIMAL(7, 2) NOT NULL DEFAULT 0,
                        {enp_processes.columns.hourly_fee} BIGINT NOT NULL DEFAULT 0,
                        {enp_processes.columns.description} NVARCHAR(MAX) DEFAULT N'',
                        CONSTRAINT fk_{tableName}_machine
                        FOREIGN KEY ({enp_processes.columns.machine_id})
                        REFERENCES {enp_Table.enp_machines}({enp_machines.columns.id})
                        ON DELETE NO ACTION
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_processes.columns.id}, {enp_processes.columns.name}, {enp_processes.columns.machine_id}, {enp_processes.columns.operators_count}, {enp_processes.columns.average_daily_production}, {enp_processes.columns.hourly_fee}, {enp_processes.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_processes.columns.name}, {enp_processes.columns.machine_id}, {enp_processes.columns.operators_count}, {enp_processes.columns.average_daily_production}, {enp_processes.columns.hourly_fee}, {enp_processes.columns.description}
                            FROM inserted
                        END;");
                }
                //Manufacturing Overhead Table
                {
                    var tableName = enp_manufacturing_overhead.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_manufacturing_overhead.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_manufacturing_overhead.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_manufacturing_overhead.columns.yearly_fee} BIGINT NOT NULL DEFAULT 0,
                        {enp_manufacturing_overhead.columns.yearly_production_capacity} INT NOT NULL DEFAULT 0,
                        {enp_manufacturing_overhead.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_manufacturing_overhead.columns.id}, {enp_manufacturing_overhead.columns.name}, {enp_manufacturing_overhead.columns.yearly_fee}, {enp_manufacturing_overhead.columns.yearly_production_capacity}, {enp_manufacturing_overhead.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_manufacturing_overhead.columns.name}, {enp_manufacturing_overhead.columns.yearly_fee}, {enp_manufacturing_overhead.columns.yearly_production_capacity}, {enp_manufacturing_overhead.columns.description}
                            FROM inserted
                        END;");
                }
                //Molds Table
                {
                    var tableName = enp_molds.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_molds.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_molds.columns.part_name} NVARCHAR(256) UNIQUE NOT NULL,
                        {enp_molds.columns.mold_id} NVARCHAR(128) NOT NULL DEFAULT N'',
                        {enp_molds.columns.tech_id} NVARCHAR(128) NOT NULL DEFAULT N'',
                        {enp_molds.columns.type} NVARCHAR(128) NOT NULL,
                        {enp_molds.columns.life_time} INT NOT NULL,
                        {enp_molds.columns.fastener_count} INT NOT NULL DEFAULT 0,
                        {enp_molds.columns.design_fee} BIGINT NOT NULL DEFAULT 0,
                        {enp_molds.columns.material_fee} BIGINT NOT NULL DEFAULT 0,
                        {enp_molds.columns.machining_fee} BIGINT NOT NULL DEFAULT 0,
                        {enp_molds.columns.other_fee} BIGINT NOT NULL DEFAULT 0,
                        {enp_molds.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_molds.columns.id}, {enp_molds.columns.part_name}, {enp_molds.columns.mold_id}, {enp_molds.columns.tech_id}, {enp_molds.columns.type}, {enp_molds.columns.life_time}, {enp_molds.columns.fastener_count}, {enp_molds.columns.design_fee}, {enp_molds.columns.material_fee}, {enp_molds.columns.machining_fee}, {enp_molds.columns.other_fee}, {enp_molds.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_molds.columns.part_name}, {enp_molds.columns.mold_id}, {enp_molds.columns.tech_id}, {enp_molds.columns.type}, {enp_molds.columns.life_time}, {enp_molds.columns.fastener_count}, {enp_molds.columns.design_fee}, {enp_molds.columns.material_fee}, {enp_molds.columns.machining_fee}, {enp_molds.columns.other_fee}, {enp_molds.columns.description}
                            FROM inserted
                        END;");
                }
                //Packing Table
                {
                    var tableName = enp_packing.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_packing.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_packing.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_packing.columns.unit_id} BIGINT NOT NULL,
                        {enp_packing.columns.price} BIGINT NOT NULL DEFAULT 0,
                        {enp_packing.columns.description} NVARCHAR(MAX) DEFAULT N'',
                        CONSTRAINT fk_{tableName}_unit
                        FOREIGN KEY ({enp_packing.columns.unit_id})
                        REFERENCES {enp_Table.enp_units}({enp_units.columns.id})
                        ON DELETE NO ACTION
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_packing.columns.id}, {enp_packing.columns.name}, {enp_packing.columns.unit_id}, {enp_packing.columns.price}, {enp_packing.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_packing.columns.name}, {enp_packing.columns.unit_id}, {enp_packing.columns.price}, {enp_packing.columns.description}
                            FROM inserted
                        END;");
                }
                //Transportation Table
                {
                    var tableName = enp_transportation.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_transportation.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_transportation.columns.name} NVARCHAR(1024) NOT NULL UNIQUE,
                        {enp_transportation.columns.price} BIGINT NOT NULL DEFAULT 0,
                        {enp_transportation.columns.description} NVARCHAR(MAX) DEFAULT N''
                        );
                        
                        GO
                        
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_transportation.columns.id}, {enp_transportation.columns.name}, {enp_transportation.columns.price}, {enp_transportation.columns.description})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_transportation.columns.name}, {enp_transportation.columns.price}, {enp_transportation.columns.description}
                            FROM inserted
                        END;");
                }
                //BOM Table
                {
                    var tableName = enp_bom.TableName;
                    tables.Add(tableName, 
                        $@"CREATE TABLE {tableName} (
                        {enp_bom.columns.id} BIGINT NOT NULL PRIMARY KEY,
                        {enp_bom.columns.product_id} BIGINT NOT NULL,
                        {enp_bom.columns.material_id} BIGINT NULL,
                        {enp_bom.columns.material_gross_consumption} DECIMAL(32, 8) NULL,
                        {enp_bom.columns.material_net_consumption} DECIMAL(32, 8) NULL,
                        {enp_bom.columns.material_part_name} NVARCHAR(1024) NULL,
                        {enp_bom.columns.part_id} BIGINT NULL,
                        {enp_bom.columns.part_consumption_coefficient} DECIMAL(32, 8) NULL,
                        {enp_bom.columns.process_id} BIGINT NULL,
                        {enp_bom.columns.process_consumption_coefficient} DECIMAL(32, 8) NULL,
                        {enp_bom.columns.mold_id} BIGINT NULL,
                        {enp_bom.columns.mold_consumption_coefficient} DECIMAL(32, 8) NULL,
                        {enp_bom.columns.manufacturing_overhead_id} BIGINT NULL,
                        {enp_bom.columns.manufacturing_overhead_share_percent} DECIMAL(13, 12) NULL,
                        {enp_bom.columns.packing_id} BIGINT NULL,
                        {enp_bom.columns.packing_consumption_coefficient} DECIMAL(32, 8) NULL,
                        {enp_bom.columns.transportation_id} BIGINT NULL,
                        {enp_bom.columns.transportation_consumption_coefficient} DECIMAL(32, 8) NULL,
                        CONSTRAINT check_{tableName}_bom CHECK (
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} >= {enp_bom.columns.material_net_consumption} THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                            OR
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NOT NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                            OR
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NOT NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                            OR
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NOT NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                            OR
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} BETWEEN 0 AND 1 THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                            OR
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NOT NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                            OR
                            (
                                CASE WHEN {enp_bom.columns.material_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_gross_consumption} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.material_net_consumption} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.part_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.part_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.process_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.process_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.mold_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.mold_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.manufacturing_overhead_share_percent} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.packing_id} IS NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.packing_consumption_coefficient} IS NULL THEN 0 ELSE 1 END +
                                
                                CASE WHEN {enp_bom.columns.transportation_id} IS NOT NULL THEN 0 ELSE 1 END +
                                CASE WHEN {enp_bom.columns.transportation_consumption_coefficient} IS NOT NULL THEN 0 ELSE 1 END 
                                
                                = 0
                            )
                        ),
                        CONSTRAINT fk_{tableName}_product_id
                        FOREIGN KEY ({enp_bom.columns.product_id})
                        REFERENCES {enp_Table.enp_products}({enp_products.columns.id})
                        ON DELETE NO ACTION,
                        
                        CONSTRAINT fk_{tableName}_material_id
                        FOREIGN KEY ({enp_bom.columns.material_id})
                        REFERENCES {enp_Table.enp_material}({enp_material.columns.id})
                        ON DELETE NO ACTION,
                        
                        CONSTRAINT fk_{tableName}_part_id
                        FOREIGN KEY ({enp_bom.columns.part_id})
                        REFERENCES {enp_Table.enp_parts}({enp_parts.columns.id})
                        ON DELETE NO ACTION,

                        CONSTRAINT fk_{tableName}_process_id
                        FOREIGN KEY ({enp_bom.columns.process_id})
                        REFERENCES {enp_Table.enp_processes}({enp_processes.columns.id})
                        ON DELETE NO ACTION,
                        
                        CONSTRAINT fk_{tableName}_mold_id
                        FOREIGN KEY ({enp_bom.columns.mold_id})
                        REFERENCES {enp_Table.enp_molds}({enp_molds.columns.id})
                        ON DELETE NO ACTION,
                        
                        CONSTRAINT fk_{tableName}_manufacturing_overhead_id
                        FOREIGN KEY ({enp_bom.columns.manufacturing_overhead_id})
                        REFERENCES {enp_Table.enp_manufacturing_overhead}({enp_manufacturing_overhead.columns.id})
                        ON DELETE NO ACTION,
                        
                        CONSTRAINT fk_{tableName}_packing_id
                        FOREIGN KEY ({enp_bom.columns.packing_id})
                        REFERENCES {enp_Table.enp_packing}({enp_packing.columns.id})
                        ON DELETE NO ACTION,
                        
                        CONSTRAINT fk_{tableName}_transportation_id
                        FOREIGN KEY ({enp_bom.columns.transportation_id})
                        REFERENCES {enp_Table.enp_transportation}({enp_transportation.columns.id})
                        ON DELETE NO ACTION,
                        );
                        
                        GO
                        CREATE TRIGGER tr_{tableName}
                        ON {tableName}
                        INSTEAD OF INSERT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            INSERT INTO {tableName} ({enp_bom.columns.id}, {enp_bom.columns.product_id}, {enp_bom.columns.material_id}, {enp_bom.columns.material_gross_consumption}, {enp_bom.columns.material_net_consumption}, {enp_bom.columns.material_part_name}, {enp_bom.columns.part_id}, {enp_bom.columns.part_consumption_coefficient}, {enp_bom.columns.process_id}, {enp_bom.columns.process_consumption_coefficient}, {enp_bom.columns.mold_id}, {enp_bom.columns.mold_consumption_coefficient}, {enp_bom.columns.manufacturing_overhead_id}, {enp_bom.columns.manufacturing_overhead_share_percent}, {enp_bom.columns.packing_id}, {enp_bom.columns.packing_consumption_coefficient}, {enp_bom.columns.transportation_id}, {enp_bom.columns.transportation_consumption_coefficient})
                            OUTPUT INSERTED.*
                            SELECT NEXT VALUE FOR {sequenceName}, {enp_bom.columns.product_id}, {enp_bom.columns.material_id}, {enp_bom.columns.material_gross_consumption}, {enp_bom.columns.material_net_consumption}, {enp_bom.columns.material_part_name}, {enp_bom.columns.part_id}, {enp_bom.columns.part_consumption_coefficient}, {enp_bom.columns.process_id}, {enp_bom.columns.process_consumption_coefficient}, {enp_bom.columns.mold_id}, {enp_bom.columns.mold_consumption_coefficient}, {enp_bom.columns.manufacturing_overhead_id}, {enp_bom.columns.manufacturing_overhead_share_percent}, {enp_bom.columns.packing_id}, {enp_bom.columns.packing_consumption_coefficient}, {enp_bom.columns.transportation_id}, {enp_bom.columns.transportation_consumption_coefficient}
                            FROM inserted
                        END;
                        CREATE INDEX IX_{tableName}_{enp_bom.columns.product_id} ON {tableName}({enp_bom.columns.product_id});");
                }

                //Create Procedures
                var procedures = new Dictionary<string, string>();
                //Units
                {
                    //Get All
                    var procedureName = enp_units.procedures.procedure_enp_units_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT * FROM {enp_Table.enp_units}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Get All Compact
                    var procedureName = enp_units.procedures.procedure_enp_units_GetAllCompact.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_units.columns.id}, {enp_units.columns.name} FROM {enp_Table.enp_units}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_units.procedures.procedure_enp_units_CreateUnit.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_units.columns.name} NVARCHAR(256),
                        @{enp_units.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_units} WHERE {enp_units.columns.name} = @{enp_units.columns.name})
                            BEGIN
                                THROW 50001, 'Duplicate name detected. Unit with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_units} ({enp_units.columns.name}, {enp_units.columns.description}) VALUES (@{enp_units.columns.name}, @{enp_units.columns.description})
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_units.procedures.procedure_enp_units_DeleteUnit.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_units.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_units} WHERE {enp_units.columns.id} = @{enp_units.columns.id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_units.procedures.procedure_enp_units_UpdateUnit.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_units.columns.id} BIGINT,
                        @{enp_units.columns.name} NVARCHAR(256),
                        @{enp_units.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_units} WHERE {enp_units.columns.name} = @{enp_units.columns.name} AND NOT {enp_units.columns.id} = @{enp_units.columns.id})
                            BEGIN
                                THROW 50001, 'Duplicate name detected. Unit with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_units} SET {enp_units.columns.name} = @{enp_units.columns.name}, {enp_units.columns.description} = @{enp_units.columns.description} OUTPUT INSERTED.* WHERE {enp_units.columns.id} = @{enp_units.columns.id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                //Products
                {
                    //Get All
                    var procedureName = enp_products.procedures.procedure_enp_products_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_products.columns.id}, {enp_products.columns.name} FROM
                            {enp_products.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_products.procedures.procedure_enp_products_CreateProduct.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_products.columns.name} NVARCHAR(1024),
                        @{enp_products.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_products} WHERE {enp_products.columns.name} = @{enp_products.columns.name})
                            BEGIN
                                THROW 50011, 'Duplicate name detected. Product with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_products} ({enp_products.columns.name}, {enp_products.columns.description})
                            VALUES (@{enp_products.columns.name}, @{enp_products.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_products.procedures.procedure_enp_products_GetProduct.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_products.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_products.TableName}.*, x.*
                            FROM {enp_products.TableName} LEFT JOIN 
                                (SELECT {enp_product_ids.TableName}.{enp_product_ids.columns.product_id}, {enp_product_ids.TableName}.{enp_product_ids.columns.id} AS {enp_product_ids.GetFullColumnName(enp_product_ids.columns.id)},
                                    {enp_product_ids.TableName}.{enp_product_ids.columns.product_id_type_id} AS {enp_product_ids.GetFullColumnName(enp_product_ids.columns.product_id_type_id)},
                                    {enp_product_ids.TableName}.{enp_product_ids.columns.value} AS {enp_product_ids.GetFullColumnName(enp_product_ids.columns.value)},
                                    {enp_product_id_types.TableName}.{enp_product_id_types.columns.name} AS {enp_product_id_types.GetFullColumnName(enp_product_id_types.columns.name)}
                                FROM {enp_Table.enp_product_ids} JOIN {enp_Table.enp_product_id_types}
                                ON {enp_product_ids.TableName}.{enp_product_ids.columns.product_id_type_id} = {enp_product_id_types.TableName}.{enp_product_id_types.columns.id}
                                WHERE {enp_product_ids.TableName}.{enp_product_ids.columns.product_id} = @{enp_products.columns.id}) x
                                ON {enp_products.TableName}.{enp_products.columns.id} = x.{enp_product_ids.columns.product_id}
                            WHERE {enp_products.TableName}.{enp_products.columns.id} = @{enp_products.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_products.procedures.procedure_enp_products_UpdateProduct.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_products.columns.id} BIGINT,
                        @{enp_products.columns.name} NVARCHAR(1024),
                        @{enp_products.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_products} WHERE {enp_products.columns.name} = @{enp_products.columns.name} AND NOT {enp_products.columns.id} = @{enp_products.columns.id})
                            BEGIN
                                THROW 50011, 'Duplicate name detected. Product with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_products} SET
                            {enp_products.columns.name} = @{enp_products.columns.name},
                            {enp_products.columns.description} = @{enp_products.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_products.columns.id} = @{enp_products.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_products.procedures.procedure_enp_products_DeleteProduct.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_products.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_products} WHERE {enp_products.columns.id} = @{enp_products.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                //Product Ids
                {
                    //Get All
                    var procedureName = enp_product_ids.procedures.procedure_enp_product_ids_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_ids.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_product_ids.TableName}.* , {enp_product_id_types.TableName}.{enp_product_id_types.columns.name} AS {enp_product_id_types.GetFullColumnName(enp_product_id_types.columns.name)}
                            FROM {enp_Table.enp_product_ids} LEFT JOIN {enp_Table.enp_product_id_types}
                            ON {enp_product_ids.TableName}.{enp_product_ids.columns.product_id_type_id} = {enp_product_id_types.TableName}.{enp_product_id_types.columns.id}
                            WHERE {enp_product_ids.TableName}.{enp_product_ids.columns.product_id} = @{enp_product_ids.columns.product_id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_product_ids.procedures.procedure_enp_product_ids_CreatePID.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_ids.columns.product_id} BIGINT,
                        @{enp_product_ids.columns.product_id_type_id} BIGINT,
                        @{enp_product_ids.columns.value} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF (EXISTS (SELECT 1 FROM {enp_Table.enp_product_ids} WHERE {enp_product_ids.columns.product_id} = @{enp_product_ids.columns.product_id} AND {enp_product_ids.columns.product_id_type_id} = @{enp_product_ids.columns.product_id_type_id} ))
                            OR
                            (EXISTS (SELECT 1 FROM {enp_product_ids.TableName} WHERE {enp_product_ids.columns.product_id_type_id} = @{enp_product_ids.columns.product_id_type_id} AND {enp_product_ids.columns.value} = @{enp_product_ids.columns.value}))
                            BEGIN
                                THROW 50010, 'Duplicate id detected. Same PID from other product already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_product_ids} ({enp_product_ids.columns.product_id}, {enp_product_ids.columns.product_id_type_id}, {enp_product_ids.columns.value}) VALUES (@{enp_product_ids.columns.product_id}, @{enp_product_ids.columns.product_id_type_id}, @{enp_product_ids.columns.value})
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_product_ids.procedures.procedure_enp_product_ids_DeletePID.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_ids.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_product_ids} WHERE {enp_product_ids.columns.id} = @{enp_product_ids.columns.id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_product_ids.procedures.procedure_enp_product_ids_UpdatePID.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_ids.columns.id} BIGINT,
                        @{enp_product_ids.columns.value} NVARCHAR(256)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_product_ids} WHERE {enp_product_ids.columns.value} = @{enp_product_ids.columns.value} AND {enp_product_ids.columns.product_id_type_id} = (SELECT {enp_product_ids.columns.product_id_type_id} FROM {enp_Table.enp_product_ids} WHERE {enp_product_ids.columns.id} = @{enp_product_ids.columns.id}) AND NOT {enp_product_ids.columns.id} = @{enp_product_ids.columns.id})
                            BEGIN
                                THROW 50010, 'Duplicate id detected. Same PID for this product already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_product_ids} SET {enp_product_ids.columns.value} = @{enp_product_ids.columns.value} OUTPUT INSERTED.* WHERE {enp_product_ids.columns.id} = @{enp_product_ids.columns.id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                //Product Id Types
                {
                    //Get All
                    var procedureName = enp_product_id_types.procedures.procedure_enp_product_id_types_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT * FROM {enp_Table.enp_product_id_types}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Get All Compact
                    var procedureName = enp_product_id_types.procedures.procedure_enp_product_id_types_GetAllCompact.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_product_id_types.columns.id}, {enp_product_id_types.columns.name} FROM {enp_Table.enp_product_id_types}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_product_id_types.procedures.procedure_enp_product_id_types_CreatePID.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_id_types.columns.name} NVARCHAR(256),
                        @{enp_product_id_types.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_product_id_types} WHERE {enp_product_id_types.columns.name} = @{enp_product_id_types.columns.name})
                            BEGIN
                                THROW 50009, 'Duplicate name detected. PID with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_product_id_types} ({enp_product_id_types.columns.name}, {enp_product_id_types.columns.description}) VALUES (@{enp_product_id_types.columns.name}, @{enp_product_id_types.columns.description})
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_product_id_types.procedures.procedure_enp_product_id_types_DeletePID.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_id_types.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_product_id_types} WHERE {enp_product_id_types.columns.id} = @{enp_product_id_types.columns.id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_product_id_types.procedures.procedure_enp_product_id_types_UpdatePID.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_product_id_types.columns.id} BIGINT,
                        @{enp_product_id_types.columns.name} NVARCHAR(256),
                        @{enp_product_id_types.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_product_id_types} WHERE {enp_product_id_types.columns.name} = @{enp_product_id_types.columns.name} AND NOT {enp_product_id_types.columns.id} = @{enp_product_id_types.columns.id})
                            BEGIN
                                THROW 50009, 'Duplicate name detected. PID with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_product_id_types} SET {enp_product_id_types.columns.name} = @{enp_product_id_types.columns.name}, {enp_product_id_types.columns.description} = @{enp_product_id_types.columns.description} OUTPUT INSERTED.* WHERE {enp_product_id_types.columns.id} = @{enp_product_id_types.columns.id}
                        END;
                        ");
                    //Console.WriteLine(procedures[procedureName]);
                }
                //Material
                {
                    //Get All
                    var procedureName = enp_material.procedures.procedure_enp_material_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_material.columns.id}, {enp_material.columns.name}, {enp_material.columns.tech_id}, {enp_material.columns.price} FROM
                            {enp_material.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_material.procedures.procedure_enp_material_CreateMaterial.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_material.columns.name} NVARCHAR(1024),
                        @{enp_material.columns.tech_id} NVARCHAR(128),
                        @{enp_material.columns.unit_id} BIGINT,
                        @{enp_material.columns.price} BIGINT,
                        @{enp_material.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_material} WHERE {enp_material.columns.name} = @{enp_material.columns.name})
                            BEGIN
                                THROW 50002, 'Duplicate name detected. Material with this name already exists.', 1;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_material} WHERE {enp_material.columns.tech_id} = @{enp_material.columns.tech_id})
                            BEGIN
                                THROW 50002, 'Duplicate tech number detected. Material with this tech number already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_material} ({enp_material.columns.name}, {enp_material.columns.tech_id}, {enp_material.columns.unit_id}, {enp_material.columns.price}, {enp_material.columns.description})
                            VALUES (@{enp_material.columns.name}, @{enp_material.columns.tech_id}, @{enp_material.columns.unit_id}, @{enp_material.columns.price}, @{enp_material.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_material.procedures.procedure_enp_material_GetMaterial.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_material.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_material.TableName}.*,
                            {enp_units.TableName}.{enp_units.columns.name} AS {enp_units.GetFullColumnName(enp_units.columns.name)}
                            FROM {enp_material.TableName} LEFT JOIN {enp_units.TableName}
                            ON {enp_material.TableName}.{enp_material.columns.unit_id} = {enp_units.TableName}.{enp_units.columns.id}
                            WHERE {enp_material.TableName}.{enp_material.columns.id} = @{enp_material.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_material.procedures.procedure_enp_material_UpdateMaterial.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_material.columns.id} BIGINT,
                        @{enp_material.columns.name} NVARCHAR(1024),
                        @{enp_material.columns.tech_id} NVARCHAR(128),
                        @{enp_material.columns.unit_id} BIGINT,
                        @{enp_material.columns.price} BIGINT,
                        @{enp_material.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_material} WHERE {enp_material.columns.name} = @{enp_material.columns.name} AND NOT {enp_material.columns.id} = @{enp_material.columns.id})
                            BEGIN
                                THROW 50002, 'Duplicate name detected. Material with this name already exists.', 2;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_material} WHERE {enp_material.columns.tech_id} = @{enp_material.columns.tech_id} AND NOT {enp_material.columns.id} = @{enp_material.columns.id})
                            BEGIN
                                THROW 50002, 'Duplicate tech number detected. Material with this tech number already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_material} SET
                            {enp_material.columns.name} = @{enp_material.columns.name},
                            {enp_material.columns.tech_id} = @{enp_material.columns.tech_id},
                            {enp_material.columns.unit_id} = @{enp_material.columns.unit_id},
                            {enp_material.columns.price} = @{enp_material.columns.price},
                            {enp_material.columns.description} = @{enp_material.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_material.columns.id} = @{enp_material.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_material.procedures.procedure_enp_material_DeleteMaterial.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_material.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_material} WHERE {enp_material.columns.id} = @{enp_material.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }

                //Parts
                {
                    //Get All
                    var procedureName = enp_parts.procedures.procedure_enp_parts_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_parts.columns.id}, {enp_parts.columns.name}, {enp_parts.columns.tech_id}, {enp_parts.columns.price} FROM
                            {enp_parts.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_parts.procedures.procedure_enp_parts_CreatePart.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_parts.columns.name} NVARCHAR(1024),
                        @{enp_parts.columns.tech_id} NVARCHAR(128),
                        @{enp_parts.columns.unit_id} BIGINT,
                        @{enp_parts.columns.price} BIGINT,
                        @{enp_parts.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_parts} WHERE {enp_parts.columns.name} = @{enp_parts.columns.name})
                            BEGIN
                                THROW 50003, 'Duplicate name detected. Part with this name already exists.', 1;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_parts} WHERE {enp_parts.columns.tech_id} = @{enp_parts.columns.tech_id})
                            BEGIN
                                THROW 50003, 'Duplicate tech number detected. Part with this tech number already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_parts} ({enp_parts.columns.name}, {enp_parts.columns.tech_id}, {enp_parts.columns.unit_id}, {enp_parts.columns.price}, {enp_parts.columns.description})
                            VALUES (@{enp_parts.columns.name}, @{enp_parts.columns.tech_id}, @{enp_parts.columns.unit_id}, @{enp_parts.columns.price}, @{enp_parts.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_parts.procedures.procedure_enp_parts_GetPart.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_parts.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_parts.TableName}.*,
                            {enp_units.TableName}.{enp_units.columns.name} AS {enp_units.GetFullColumnName(enp_units.columns.name)}
                            FROM {enp_parts.TableName} LEFT JOIN {enp_units.TableName}
                            ON {enp_parts.TableName}.{enp_parts.columns.unit_id} = {enp_units.TableName}.{enp_units.columns.id}
                            WHERE {enp_parts.TableName}.{enp_parts.columns.id} = @{enp_parts.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_parts.procedures.procedure_enp_parts_UpdatePart.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_parts.columns.id} BIGINT,
                        @{enp_parts.columns.name} NVARCHAR(1024),
                        @{enp_parts.columns.tech_id} NVARCHAR(128),
                        @{enp_parts.columns.unit_id} BIGINT,
                        @{enp_parts.columns.price} BIGINT,
                        @{enp_parts.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_parts} WHERE {enp_parts.columns.name} = @{enp_parts.columns.name} AND NOT {enp_parts.columns.id} = @{enp_parts.columns.id})
                            BEGIN
                                THROW 50003, 'Duplicate name detected. Part with this name already exists.', 2;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_parts} WHERE {enp_parts.columns.tech_id} = @{enp_parts.columns.tech_id} AND NOT {enp_parts.columns.id} = @{enp_parts.columns.id})
                            BEGIN
                                THROW 50003, 'Duplicate tech number detected. Part with this tech number already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_parts} SET
                            {enp_parts.columns.name} = @{enp_parts.columns.name},
                            {enp_parts.columns.tech_id} = @{enp_parts.columns.tech_id},
                            {enp_parts.columns.unit_id} = @{enp_parts.columns.unit_id},
                            {enp_parts.columns.price} = @{enp_parts.columns.price},
                            {enp_parts.columns.description} = @{enp_parts.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_parts.columns.id} = @{enp_parts.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_parts.procedures.procedure_enp_parts_DeletePart.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_parts.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_parts} WHERE {enp_parts.columns.id} = @{enp_parts.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }

                //Machines
                {
                    //GetAll
                    var procedureName = enp_machines.procedures.procedure_enp_machines_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_machines.columns.id}, {enp_machines.columns.name} FROM
                            {enp_machines.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_machines.procedures.procedure_enp_machines_GetMachine.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_machines.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT *
                            FROM {enp_machines.TableName}
                            WHERE {enp_machines.columns.id} = @{enp_machines.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_machines.procedures.procedure_enp_machines_CreateMachine.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_machines.columns.name} NVARCHAR(1024),
                        @{enp_machines.columns.working_days_per_year} DECIMAL(4, 1),
                        @{enp_machines.columns.working_hours_daily} DECIMAL(3, 1),
                        @{enp_machines.columns.life_time} DECIMAL(6, 3),
                        @{enp_machines.columns.price_per_uint} BIGINT,
                        @{enp_machines.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_machines} WHERE {enp_machines.columns.name} = @{enp_machines.columns.name})
                            BEGIN
                                THROW 50004, 'Duplicate name detected. Machine with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_machines} ({enp_machines.columns.name}, {enp_machines.columns.working_days_per_year}, {enp_machines.columns.working_hours_daily}, {enp_machines.columns.life_time}, {enp_machines.columns.price_per_uint}, {enp_machines.columns.description})
                            VALUES (@{enp_machines.columns.name}, @{enp_machines.columns.working_days_per_year}, @{enp_machines.columns.working_hours_daily}, @{enp_machines.columns.life_time}, @{enp_machines.columns.price_per_uint}, @{enp_machines.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_machines.procedures.procedure_enp_machines_UpdateMachine.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_machines.columns.id} BIGINT,
                        @{enp_machines.columns.name} NVARCHAR(1024),
                        @{enp_machines.columns.working_days_per_year} DECIMAL(4, 1),
                        @{enp_machines.columns.working_hours_daily} DECIMAL(3, 1),
                        @{enp_machines.columns.life_time} DECIMAL(6, 3),
                        @{enp_machines.columns.price_per_uint} BIGINT,
                        @{enp_machines.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_machines} WHERE {enp_machines.columns.name} = @{enp_machines.columns.name} AND NOT {enp_machines.columns.id} = @{enp_machines.columns.id})
                            BEGIN
                                THROW 50004, 'Duplicate name detected. Machine with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_machines} SET
                            {enp_machines.columns.name} = @{enp_machines.columns.name},
                            {enp_machines.columns.working_days_per_year} = @{enp_machines.columns.working_days_per_year},
                            {enp_machines.columns.working_hours_daily} = @{enp_machines.columns.working_hours_daily},
                            {enp_machines.columns.life_time} = @{enp_machines.columns.life_time},
                            {enp_machines.columns.price_per_uint} = @{enp_machines.columns.price_per_uint},
                            {enp_machines.columns.description} = @{enp_machines.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_machines.columns.id} = @{enp_machines.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_machines.procedures.procedure_enp_machines_DeleteMachine.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_machines.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_machines} WHERE {enp_machines.columns.id} = @{enp_machines.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }

                //Processes
                {
                    //GetAll
                    var procedureName = enp_processes.procedures.procedure_enp_processes_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_processes.columns.id}, {enp_processes.columns.name} FROM
                            {enp_processes.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_processes.procedures.procedure_enp_processes_GetProcess.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_processes.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_processes.TableName}.*,
                            {enp_machines.TableName}.{enp_machines.columns.name} AS {enp_machines.GetFullColumnName(enp_machines.columns.name)}
                            FROM {enp_processes.TableName} LEFT JOIN {enp_machines.TableName}
                            ON {enp_processes.TableName}.{enp_processes.columns.machine_id} = {enp_machines.TableName}.{enp_machines.columns.id}
                            WHERE {enp_processes.TableName}.{enp_processes.columns.id} = @{enp_processes.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_processes.procedures.procedure_enp_processes_CreateProcess.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_processes.columns.name} NVARCHAR(1024),
                        @{enp_processes.columns.machine_id} BIGINT,
                        @{enp_processes.columns.operators_count} INT,
                        @{enp_processes.columns.average_daily_production} DECIMAL(7, 2),
                        @{enp_processes.columns.hourly_fee} BIGINT,
                        @{enp_processes.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_processes} WHERE {enp_processes.columns.name} = @{enp_processes.columns.name})
                            BEGIN
                                THROW 50005, 'Duplicate name detected. Process with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_processes} ({enp_processes.columns.name}, {enp_processes.columns.machine_id}, {enp_processes.columns.operators_count}, {enp_processes.columns.average_daily_production}, {enp_processes.columns.hourly_fee}, {enp_processes.columns.description})
                            VALUES (@{enp_processes.columns.name}, @{enp_processes.columns.machine_id}, @{enp_processes.columns.operators_count}, @{enp_processes.columns.average_daily_production}, @{enp_processes.columns.hourly_fee}, @{enp_processes.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_processes.procedures.procedure_enp_processes_UpdateProcess.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_processes.columns.id} BIGINT,
                        @{enp_processes.columns.name} NVARCHAR(1024),
                        @{enp_processes.columns.machine_id} BIGINT,
                        @{enp_processes.columns.operators_count} INT,
                        @{enp_processes.columns.average_daily_production} DECIMAL(7, 2),
                        @{enp_processes.columns.hourly_fee} BIGINT,
                        @{enp_processes.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_processes} WHERE {enp_processes.columns.name} = @{enp_processes.columns.name} AND NOT {enp_processes.columns.id} = @{enp_processes.columns.id})
                            BEGIN
                                THROW 50005, 'Duplicate name detected. Process with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_processes} SET
                            {enp_processes.columns.name} = @{enp_machines.columns.name},
                            {enp_processes.columns.machine_id} = @{enp_processes.columns.machine_id},
                            {enp_processes.columns.operators_count} = @{enp_processes.columns.operators_count},
                            {enp_processes.columns.average_daily_production} = @{enp_processes.columns.average_daily_production},
                            {enp_processes.columns.hourly_fee} = @{enp_processes.columns.hourly_fee},
                            {enp_processes.columns.description} = @{enp_processes.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_processes.columns.id} = @{enp_processes.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_processes.procedures.procedure_enp_processes_DeleteProcess.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_processes.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_processes} WHERE {enp_processes.columns.id} = @{enp_processes.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }

                //Manufacturing Overhead
                {
                    //GetAll
                    var procedureName = enp_manufacturing_overhead.procedures.procedure_enp_manufacturing_overhead_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_manufacturing_overhead.columns.id}, {enp_manufacturing_overhead.columns.name} FROM
                            {enp_manufacturing_overhead.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_manufacturing_overhead.procedures.procedure_enp_manufacturing_overhead_GetManufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_manufacturing_overhead.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT *
                            FROM {enp_manufacturing_overhead.TableName}
                            WHERE {enp_manufacturing_overhead.columns.id} = @{enp_manufacturing_overhead.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_manufacturing_overhead.procedures.procedure_enp_manufacturing_overhead_CreateManufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_manufacturing_overhead.columns.name} NVARCHAR(1024),
                        @{enp_manufacturing_overhead.columns.yearly_fee} BIGINT,
                        @{enp_manufacturing_overhead.columns.yearly_production_capacity} INT,
                        @{enp_manufacturing_overhead.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_manufacturing_overhead} WHERE {enp_manufacturing_overhead.columns.name} = @{enp_manufacturing_overhead.columns.name})
                            BEGIN
                                THROW 50005, 'Duplicate name detected. Manufacturing Overhead with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_manufacturing_overhead} ({enp_manufacturing_overhead.columns.name}, {enp_manufacturing_overhead.columns.yearly_fee}, {enp_manufacturing_overhead.columns.yearly_production_capacity}, {enp_manufacturing_overhead.columns.description})
                            VALUES (@{enp_manufacturing_overhead.columns.name}, @{enp_manufacturing_overhead.columns.yearly_fee}, @{enp_manufacturing_overhead.columns.yearly_production_capacity}, @{enp_manufacturing_overhead.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_manufacturing_overhead.procedures.procedure_enp_manufacturing_overhead_UpdateManufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_manufacturing_overhead.columns.id} BIGINT,
                        @{enp_manufacturing_overhead.columns.name} NVARCHAR(1024),
                        @{enp_manufacturing_overhead.columns.yearly_fee} BIGINT,
                        @{enp_manufacturing_overhead.columns.yearly_production_capacity} INT,
                        @{enp_manufacturing_overhead.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_manufacturing_overhead} WHERE {enp_manufacturing_overhead.columns.name} = @{enp_manufacturing_overhead.columns.name} AND NOT {enp_manufacturing_overhead.columns.id} = @{enp_manufacturing_overhead.columns.id})
                            BEGIN
                                THROW 50005, 'Duplicate name detected. Manufacturing Overhead with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_manufacturing_overhead} SET
                            {enp_manufacturing_overhead.columns.name} = @{enp_manufacturing_overhead.columns.name},
                            {enp_manufacturing_overhead.columns.yearly_fee} = @{enp_manufacturing_overhead.columns.yearly_fee},
                            {enp_manufacturing_overhead.columns.yearly_production_capacity} = @{enp_manufacturing_overhead.columns.yearly_production_capacity},
                            {enp_manufacturing_overhead.columns.description} = @{enp_manufacturing_overhead.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_manufacturing_overhead.columns.id} = @{enp_manufacturing_overhead.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_manufacturing_overhead.procedures.procedure_enp_manufacturing_overhead_DeleteManufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_manufacturing_overhead.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_manufacturing_overhead} WHERE {enp_manufacturing_overhead.columns.id} = @{enp_manufacturing_overhead.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                //Molds
                {
                    //GetAll
                    var procedureName = enp_molds.procedures.procedure_enp_molds_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_molds.columns.id}, {enp_molds.columns.part_name}, {enp_molds.columns.mold_id}, {enp_molds.columns.tech_id} FROM
                            {enp_molds.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_molds.procedures.procedure_enp_molds_GetMold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_molds.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT *
                            FROM {enp_molds.TableName}
                            WHERE {enp_molds.columns.id} = @{enp_molds.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_molds.procedures.procedure_enp_molds_CreateMold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_molds.columns.part_name} NVARCHAR(256),
                        @{enp_molds.columns.mold_id} NVARCHAR(128),
                        @{enp_molds.columns.tech_id} NVARCHAR(128),
                        @{enp_molds.columns.type} NVARCHAR(128),
                        @{enp_molds.columns.life_time} INT,
                        @{enp_molds.columns.fastener_count} INT,
                        @{enp_molds.columns.design_fee} BIGINT,
                        @{enp_molds.columns.material_fee} BIGINT,
                        @{enp_molds.columns.machining_fee} BIGINT,
                        @{enp_molds.columns.other_fee} BIGINT,
                        @{enp_molds.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.part_name} = @{enp_molds.columns.part_name})
                            BEGIN
                                THROW 50006, 'Duplicate name detected. Mold with this part name already exists.', 1;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.tech_id} = @{enp_molds.columns.tech_id})
                            BEGIN
                                THROW 50006, 'Duplicate name detected. Mold with this tech number already exists.', 1;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.mold_id} = @{enp_molds.columns.mold_id})
                            BEGIN
                                THROW 50006, 'Duplicate name detected. Mold with this id already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_molds} ({enp_molds.columns.part_name}, {enp_molds.columns.mold_id}, {enp_molds.columns.tech_id}, {enp_molds.columns.type}, {enp_molds.columns.life_time}, {enp_molds.columns.fastener_count}, {enp_molds.columns.design_fee}, {enp_molds.columns.material_fee}, {enp_molds.columns.machining_fee}, {enp_molds.columns.other_fee}, {enp_molds.columns.description})
                            VALUES (@{enp_molds.columns.part_name}, @{enp_molds.columns.mold_id}, @{enp_molds.columns.tech_id}, @{enp_molds.columns.type}, @{enp_molds.columns.life_time}, @{enp_molds.columns.fastener_count}, @{enp_molds.columns.design_fee}, @{enp_molds.columns.material_fee}, @{enp_molds.columns.machining_fee}, @{enp_molds.columns.other_fee}, @{enp_molds.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_molds.procedures.procedure_enp_molds_UpdateMold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_molds.columns.id} BIGINT,
                        @{enp_molds.columns.part_name} NVARCHAR(256),
                        @{enp_molds.columns.mold_id} NVARCHAR(128),
                        @{enp_molds.columns.tech_id} NVARCHAR(128),
                        @{enp_molds.columns.type} NVARCHAR(128),
                        @{enp_molds.columns.life_time} INT,
                        @{enp_molds.columns.fastener_count} INT,
                        @{enp_molds.columns.design_fee} BIGINT,
                        @{enp_molds.columns.material_fee} BIGINT,
                        @{enp_molds.columns.machining_fee} BIGINT,
                        @{enp_molds.columns.other_fee} BIGINT,
                        @{enp_molds.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.part_name} = @{enp_molds.columns.part_name} AND NOT {enp_molds.columns.id} = @{enp_molds.columns.id})
                            BEGIN
                                THROW 50006, 'Duplicate name detected. Mold with this part name already exists.', 2;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.tech_id} = @{enp_molds.columns.tech_id} AND NOT {enp_molds.columns.id} = @{enp_molds.columns.id})
                            BEGIN
                                THROW 50006, 'Duplicate name detected. Mold with this tech number already exists.', 2;
                            END;
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.mold_id} = @{enp_molds.columns.mold_id} AND NOT {enp_molds.columns.id} = @{enp_molds.columns.id})
                            BEGIN
                                THROW 50006, 'Duplicate name detected. Mold with this id already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_molds} SET
                            {enp_molds.columns.part_name} = @{enp_molds.columns.part_name},
                            {enp_molds.columns.mold_id} = @{enp_molds.columns.mold_id},
                            {enp_molds.columns.tech_id} = @{enp_molds.columns.tech_id},
                            {enp_molds.columns.type} = @{enp_molds.columns.type},
                            {enp_molds.columns.life_time} = @{enp_molds.columns.life_time},
                            {enp_molds.columns.fastener_count} = @{enp_molds.columns.fastener_count},
                            {enp_molds.columns.design_fee} = @{enp_molds.columns.design_fee},
                            {enp_molds.columns.material_fee} = @{enp_molds.columns.material_fee},
                            {enp_molds.columns.machining_fee} = @{enp_molds.columns.machining_fee},
                            {enp_molds.columns.other_fee} = @{enp_molds.columns.other_fee},
                            {enp_molds.columns.description} = @{enp_molds.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_molds.columns.id} = @{enp_molds.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_molds.procedures.procedure_enp_molds_DeleteMold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_molds.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_molds} WHERE {enp_molds.columns.id} = @{enp_molds.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                //Packing
                {
                    //Get All
                    var procedureName = enp_packing.procedures.procedure_enp_packing_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_packing.columns.id}, {enp_packing.columns.name}, {enp_packing.columns.price} FROM
                            {enp_packing.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_packing.procedures.procedure_enp_packing_CreatePacking.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_packing.columns.name} NVARCHAR(1024),
                        @{enp_packing.columns.unit_id} BIGINT,
                        @{enp_packing.columns.price} BIGINT,
                        @{enp_packing.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_packing} WHERE {enp_packing.columns.name} = @{enp_packing.columns.name})
                            BEGIN
                                THROW 50007, 'Duplicate name detected. Packing with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_packing} ({enp_packing.columns.name}, {enp_packing.columns.unit_id}, {enp_packing.columns.price}, {enp_packing.columns.description})
                            VALUES (@{enp_packing.columns.name}, @{enp_packing.columns.unit_id}, @{enp_packing.columns.price}, @{enp_packing.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_packing.procedures.procedure_enp_packing_GetPacking.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_packing.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_packing.TableName}.*,
                            {enp_units.TableName}.{enp_units.columns.name} AS {enp_units.GetFullColumnName(enp_units.columns.name)}
                            FROM {enp_packing.TableName} LEFT JOIN {enp_units.TableName}
                            ON {enp_packing.TableName}.{enp_packing.columns.unit_id} = {enp_units.TableName}.{enp_units.columns.id}
                            WHERE {enp_packing.TableName}.{enp_packing.columns.id} = @{enp_packing.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_packing.procedures.procedure_enp_packing_UpdatePacking.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_packing.columns.id} BIGINT,
                        @{enp_packing.columns.name} NVARCHAR(1024),
                        @{enp_packing.columns.unit_id} BIGINT,
                        @{enp_packing.columns.price} BIGINT,
                        @{enp_packing.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_packing} WHERE {enp_packing.columns.name} = @{enp_packing.columns.name} AND NOT {enp_packing.columns.id} = @{enp_packing.columns.id})
                            BEGIN
                                THROW 50007, 'Duplicate name detected. Packing with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_packing} SET
                            {enp_packing.columns.name} = @{enp_packing.columns.name},
                            {enp_packing.columns.unit_id} = @{enp_packing.columns.unit_id},
                            {enp_packing.columns.price} = @{enp_packing.columns.price},
                            {enp_packing.columns.description} = @{enp_packing.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_packing.columns.id} = @{enp_packing.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_packing.procedures.procedure_enp_packing_DeletePacking.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_packing.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_packing} WHERE {enp_packing.columns.id} = @{enp_packing.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                //Transportation
                {
                    //Get All
                    var procedureName = enp_transportation.procedures.procedure_enp_transportation_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        AS
                        BEGIN
                            SELECT {enp_transportation.columns.id}, {enp_transportation.columns.name}, {enp_transportation.columns.price} FROM
                            {enp_transportation.TableName};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Create
                    var procedureName = enp_transportation.procedures.procedure_enp_transportation_CreateTransportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_transportation.columns.name} NVARCHAR(1024),
                        @{enp_transportation.columns.price} BIGINT,
                        @{enp_transportation.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_transportation} WHERE {enp_transportation.columns.name} = @{enp_transportation.columns.name})
                            BEGIN
                                THROW 50008, 'Duplicate name detected. Transportation with this name already exists.', 1;
                            END;
                            INSERT INTO {enp_Table.enp_transportation} ({enp_transportation.columns.name}, {enp_transportation.columns.price}, {enp_transportation.columns.description})
                            VALUES (@{enp_transportation.columns.name}, @{enp_transportation.columns.price}, @{enp_transportation.columns.description})
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Get
                    var procedureName = enp_transportation.procedures.procedure_enp_transportation_GetTransportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_transportation.columns.id} BIGINT
                        AS
                        BEGIN
                            SELECT *
                            FROM {enp_transportation.TableName}
                            WHERE {enp_transportation.columns.id} = @{enp_transportation.columns.id};
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Update
                    var procedureName = enp_transportation.procedures.procedure_enp_transportation_UpdateTransportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_transportation.columns.id} BIGINT,
                        @{enp_transportation.columns.name} NVARCHAR(1024),
                        @{enp_transportation.columns.price} BIGINT,
                        @{enp_transportation.columns.description} NVARCHAR(MAX)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_Table.enp_transportation} WHERE {enp_transportation.columns.name} = @{enp_transportation.columns.name} AND NOT {enp_transportation.columns.id} = @{enp_transportation.columns.id})
                            BEGIN
                                THROW 50008, 'Duplicate name detected. Transportation with this name already exists.', 2;
                            END;
                            UPDATE {enp_Table.enp_transportation} SET
                            {enp_transportation.columns.name} = @{enp_transportation.columns.name},
                            {enp_transportation.columns.price} = @{enp_transportation.columns.price},
                            {enp_transportation.columns.description} = @{enp_transportation.columns.description}
                            OUTPUT INSERTED.*
                            WHERE {enp_transportation.columns.id} = @{enp_transportation.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                {
                    //Delete
                    var procedureName = enp_transportation.procedures.procedure_enp_transportation_DeleteTransportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_transportation.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_Table.enp_transportation} WHERE {enp_transportation.columns.id} = @{enp_transportation.columns.id}
                        END;
                        ");
                    //Debug.WriteLine(procedures[procedureName]);
                }
                //BOM
                {
                    //Get All
                    var procedureName = enp_bom.procedures.procedure_enp_bom_GetAll.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.*, {enp_material.TableName}.{enp_material.columns.name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_material.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.material_id} = {enp_material.TableName}.{enp_material.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}

                            UNION

                            SELECT {enp_bom.TableName}.*, {enp_parts.TableName}.{enp_parts.columns.name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_parts.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.part_id} = {enp_parts.TableName}.{enp_parts.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}

                            UNION

                            SELECT {enp_bom.TableName}.*, {enp_processes.TableName}.{enp_processes.columns.name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_processes.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.process_id} = {enp_processes.TableName}.{enp_processes.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}

                            UNION

                            SELECT {enp_bom.TableName}.*, {enp_molds.TableName}.{enp_molds.columns.part_name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_molds.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.mold_id} = {enp_molds.TableName}.{enp_molds.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}

                            UNION

                            SELECT {enp_bom.TableName}.*, {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_manufacturing_overhead.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.manufacturing_overhead_id} = {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}

                            UNION

                            SELECT {enp_bom.TableName}.*, {enp_packing.TableName}.{enp_packing.columns.name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_packing.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.packing_id} = {enp_packing.TableName}.{enp_packing.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}

                            UNION

                            SELECT {enp_bom.TableName}.*, {enp_transportation.TableName}.{enp_transportation.columns.name} AS name
                            FROM {enp_Table.enp_bom} JOIN {enp_transportation.TableName} 
                            ON {enp_bom.TableName}.{enp_bom.columns.transportation_id} = {enp_transportation.TableName}.{enp_transportation.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id}
                        END;
                        ");
                }
                {
                    //Create Material
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreateMaterial.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.material_id} BIGINT,
                        @{enp_bom.columns.material_net_consumption} DECIMAL(32, 8),
                        @{enp_bom.columns.material_gross_consumption} DECIMAL(32, 8),
                        @{enp_bom.columns.material_part_name} NVARCHAR(1024)
                        AS
                        BEGIN
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.material_id}, {enp_bom.columns.material_net_consumption}, {enp_bom.columns.material_gross_consumption}, {enp_bom.columns.material_part_name})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.material_id}, @{enp_bom.columns.material_net_consumption}, @{enp_bom.columns.material_gross_consumption}, @{enp_bom.columns.material_part_name})
                        END;
                        ");
                }
                {
                    //Update Material
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdateMaterial.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.material_id} BIGINT,
                        @{enp_bom.columns.material_net_consumption} DECIMAL(32, 8),
                        @{enp_bom.columns.material_gross_consumption} DECIMAL(32, 8),
                        @{enp_bom.columns.material_part_name} NVARCHAR(1024)
                        AS
                        BEGIN
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.material_id} = @{enp_bom.columns.material_id},
                            {enp_bom.columns.material_net_consumption} = @{enp_bom.columns.material_net_consumption},
                            {enp_bom.columns.material_gross_consumption} = @{enp_bom.columns.material_gross_consumption},
                            {enp_bom.columns.material_part_name} = @{enp_bom.columns.material_part_name}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Create Part
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreatePart.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.part_id} BIGINT,
                        @{enp_bom.columns.part_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.part_id} = @{enp_bom.columns.part_id})
                            BEGIN
                                THROW 50013, 'Duplicate item detected. Item with this part already exists.', 1;
                            END;
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.part_id}, {enp_bom.columns.part_consumption_coefficient})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.part_id}, @{enp_bom.columns.part_consumption_coefficient})
                        END;
                        ");
                }
                {
                    //Update Part
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdatePart.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.part_id} BIGINT,
                        @{enp_bom.columns.part_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.part_id} = @{enp_bom.columns.part_id})
                            BEGIN
                                THROW 50013, 'Duplicate item detected. Item with this part already exists.', 2;
                            END;
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.part_id} = @{enp_bom.columns.part_id},
                            {enp_bom.columns.part_consumption_coefficient} = @{enp_bom.columns.part_consumption_coefficient}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Create Process
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreateProcess.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.process_id} BIGINT,
                        @{enp_bom.columns.process_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.process_id} = @{enp_bom.columns.process_id})
                            BEGIN
                                THROW 50014, 'Duplicate item detected. Item with this process already exists.', 1;
                            END;
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.process_id}, {enp_bom.columns.process_consumption_coefficient})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.process_id}, @{enp_bom.columns.process_consumption_coefficient})
                        END;
                        ");
                }
                {
                    //Update Process
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdateProcess.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.process_id} BIGINT,
                        @{enp_bom.columns.process_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.process_id} = @{enp_bom.columns.process_id})
                            BEGIN
                                THROW 50014, 'Duplicate item detected. Item with this process already exists.', 2;
                            END;
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.process_id} = @{enp_bom.columns.process_id},
                            {enp_bom.columns.process_consumption_coefficient} = @{enp_bom.columns.process_consumption_coefficient}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Create Manufacturing overhead
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreateManufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.manufacturing_overhead_id} BIGINT,
                        @{enp_bom.columns.manufacturing_overhead_share_percent} DECIMAL(13, 12)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.manufacturing_overhead_id} = @{enp_bom.columns.manufacturing_overhead_id})
                            BEGIN
                                THROW 50015, 'Duplicate item detected. Item with this maunfacturing overhead already exists.', 1;
                            END;
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.manufacturing_overhead_id}, {enp_bom.columns.manufacturing_overhead_share_percent})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.manufacturing_overhead_id}, @{enp_bom.columns.manufacturing_overhead_share_percent})
                        END;
                        ");
                }
                {
                    //Update Manufacturing overhead
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdateManufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.manufacturing_overhead_id} BIGINT,
                        @{enp_bom.columns.manufacturing_overhead_share_percent} DECIMAL(13, 12)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.manufacturing_overhead_id} = @{enp_bom.columns.manufacturing_overhead_id})
                            BEGIN
                                THROW 50015, 'Duplicate item detected. Item with this maunfacturing overhead already exists.', 2;
                            END;
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.manufacturing_overhead_id} = @{enp_bom.columns.manufacturing_overhead_id},
                            {enp_bom.columns.manufacturing_overhead_share_percent} = @{enp_bom.columns.manufacturing_overhead_share_percent}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Create Mold
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreateMold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.mold_id} BIGINT,
                        @{enp_bom.columns.mold_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.mold_id} = @{enp_bom.columns.mold_id})
                            BEGIN
                                THROW 50016, 'Duplicate item detected. Item with this mold already exists.', 1;
                            END;
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.mold_id}, {enp_bom.columns.mold_consumption_coefficient})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.mold_id}, @{enp_bom.columns.mold_consumption_coefficient})
                        END;
                        ");
                }
                {
                    //Update Mold
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdateMold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.mold_id} BIGINT,
                        @{enp_bom.columns.mold_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.mold_id} = @{enp_bom.columns.mold_id})
                            BEGIN
                                THROW 50016, 'Duplicate item detected. Item with this mold already exists.', 2;
                            END;
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.mold_id} = @{enp_bom.columns.mold_id},
                            {enp_bom.columns.mold_consumption_coefficient} = @{enp_bom.columns.mold_consumption_coefficient}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Create Packing
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreatePacking.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.packing_id} BIGINT,
                        @{enp_bom.columns.packing_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.packing_id} = @{enp_bom.columns.packing_id})
                            BEGIN
                                THROW 50017, 'Duplicate item detected. Item with this packing already exists.', 1;
                            END;
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.packing_id}, {enp_bom.columns.packing_consumption_coefficient})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.packing_id}, @{enp_bom.columns.packing_consumption_coefficient})
                        END;
                        ");
                }
                {
                    //Update Packing
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdatePacking.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.packing_id} BIGINT,
                        @{enp_bom.columns.packing_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.packing_id} = @{enp_bom.columns.packing_id})
                            BEGIN
                                THROW 50017, 'Duplicate item detected. Item with this packing already exists.', 2;
                            END;
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.packing_id} = @{enp_bom.columns.packing_id},
                            {enp_bom.columns.packing_consumption_coefficient} = @{enp_bom.columns.packing_consumption_coefficient}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Create Transportation
                    var procedureName = enp_bom.procedures.procedure_enp_bom_CreateTransportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.transportation_id} BIGINT,
                        @{enp_bom.columns.transportation_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.transportation_id} = @{enp_bom.columns.transportation_id})
                            BEGIN
                                THROW 50018, 'Duplicate item detected. Item with this transportation already exists.', 1;
                            END;
                            INSERT INTO {enp_bom.TableName} ({enp_bom.columns.product_id}, {enp_bom.columns.transportation_id}, {enp_bom.columns.transportation_consumption_coefficient})
                            VALUES (@{enp_bom.columns.product_id}, @{enp_bom.columns.transportation_id}, @{enp_bom.columns.transportation_consumption_coefficient})
                        END;
                        ");
                }
                {
                    //Update Transportation
                    var procedureName = enp_bom.procedures.procedure_enp_bom_UpdateTransportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT,
                        @{enp_bom.columns.product_id} BIGINT,
                        @{enp_bom.columns.transportation_id} BIGINT,
                        @{enp_bom.columns.transportation_consumption_coefficient} DECIMAL(32, 8)
                        AS
                        BEGIN
                            IF EXISTS (SELECT 1 FROM {enp_bom.TableName} WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.transportation_id} = @{enp_bom.columns.transportation_id})
                            BEGIN
                                THROW 50018, 'Duplicate item detected. Item with this transportation already exists.', 2;
                            END;
                            UPDATE {enp_bom.TableName} SET
                            {enp_bom.columns.transportation_id} = @{enp_bom.columns.transportation_id},
                            {enp_bom.columns.transportation_consumption_coefficient} = @{enp_bom.columns.transportation_consumption_coefficient}
                            OUTPUT INSERTED.*
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //Delete BOM
                    var procedureName = enp_bom.procedures.procedure_enp_bom_DeleteBOM.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.id} BIGINT
                        AS
                        BEGIN
                            DELETE FROM {enp_bom.TableName}
                            WHERE {enp_bom.columns.id} = @{enp_bom.columns.id}
                        END;
                        ");
                }
                {
                    //BOM Material Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_material.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.material_net_consumption},
                                {enp_bom.TableName}.{enp_bom.columns.material_gross_consumption},
                                {enp_bom.TableName}.{enp_bom.columns.material_part_name},
                                x.*
                            FROM {enp_bom.TableName} LEFT JOIN
                                (SELECT {enp_material.TableName}.{enp_material.columns.name} AS {enp_material.GetFullColumnName(enp_material.columns.name)},
                                    {enp_material.TableName}.{enp_material.columns.price} AS {enp_material.GetFullColumnName(enp_material.columns.price)},
                                    {enp_material.TableName}.{enp_material.columns.tech_id} AS {enp_material.GetFullColumnName(enp_material.columns.tech_id)},
                                    {enp_material.TableName}.{enp_material.columns.id} AS {enp_material.GetFullColumnName(enp_material.columns.id)},
                                    {enp_units.TableName}.{enp_units.columns.name} AS {enp_units.GetFullColumnName(enp_units.columns.name)}
                                FROM {enp_material.TableName} LEFT JOIN {enp_units.TableName}
                                ON {enp_material.TableName}.{enp_material.columns.unit_id} = {enp_units.TableName}.{enp_units.columns.id}) x
                            ON {enp_bom.TableName}.{enp_bom.columns.material_id} = x.{enp_material.GetFullColumnName(enp_material.columns.id)}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.material_id} IS NOT NULL
                                
                        END;
                        ");
                }
                {
                    //BOM Part Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_part.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.part_consumption_coefficient},
                                x.*
                            FROM {enp_bom.TableName} LEFT JOIN
                                (SELECT {enp_parts.TableName}.{enp_parts.columns.name} AS {enp_parts.GetFullColumnName(enp_parts.columns.name)},
                                    {enp_parts.TableName}.{enp_parts.columns.price} AS {enp_parts.GetFullColumnName(enp_parts.columns.price)},
                                    {enp_parts.TableName}.{enp_parts.columns.tech_id} AS {enp_parts.GetFullColumnName(enp_parts.columns.tech_id)},
                                    {enp_parts.TableName}.{enp_parts.columns.id} AS {enp_parts.GetFullColumnName(enp_parts.columns.id)},
                                    {enp_units.TableName}.{enp_units.columns.name} AS {enp_units.GetFullColumnName(enp_units.columns.name)}
                                FROM {enp_parts.TableName} LEFT JOIN {enp_units.TableName}
                                ON {enp_parts.TableName}.{enp_parts.columns.unit_id} = {enp_units.TableName}.{enp_units.columns.id}) x
                            ON {enp_bom.TableName}.{enp_bom.columns.part_id} = x.{enp_parts.GetFullColumnName(enp_parts.columns.id)}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.part_id} IS NOT NULL
                                
                        END;
                        ");
                }
                {
                    //BOM Process Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_process.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.process_consumption_coefficient},
                                x.*
                            FROM {enp_bom.TableName} LEFT JOIN
                                (SELECT {enp_processes.TableName}.{enp_processes.columns.name} AS {enp_processes.GetFullColumnName(enp_processes.columns.name)},
                                    {enp_processes.TableName}.{enp_processes.columns.operators_count} AS {enp_processes.GetFullColumnName(enp_processes.columns.operators_count)},
                                    {enp_processes.TableName}.{enp_processes.columns.average_daily_production} AS {enp_processes.GetFullColumnName(enp_processes.columns.average_daily_production)},
                                    {enp_processes.TableName}.{enp_processes.columns.hourly_fee} AS {enp_processes.GetFullColumnName(enp_processes.columns.hourly_fee)},
                                    {enp_processes.TableName}.{enp_processes.columns.id} AS {enp_processes.GetFullColumnName(enp_processes.columns.id)},
                                    {enp_machines.TableName}.{enp_machines.columns.name} AS {enp_machines.GetFullColumnName(enp_machines.columns.name)},
                                    {enp_machines.TableName}.{enp_machines.columns.working_days_per_year} AS {enp_machines.GetFullColumnName(enp_machines.columns.working_days_per_year)},
                                    {enp_machines.TableName}.{enp_machines.columns.working_hours_daily} AS {enp_machines.GetFullColumnName(enp_machines.columns.working_hours_daily)},
                                    {enp_machines.TableName}.{enp_machines.columns.price_per_uint} AS {enp_machines.GetFullColumnName(enp_machines.columns.price_per_uint)},
                                    {enp_machines.TableName}.{enp_machines.columns.life_time} AS {enp_machines.GetFullColumnName(enp_machines.columns.life_time)},
                                    {enp_machines.TableName}.{enp_machines.columns.id} AS {enp_machines.GetFullColumnName(enp_machines.columns.id)}
                                FROM {enp_processes.TableName} LEFT JOIN {enp_machines.TableName}
                                ON {enp_processes.TableName}.{enp_processes.columns.machine_id} = {enp_machines.TableName}.{enp_machines.columns.id}) x
                            ON {enp_bom.TableName}.{enp_bom.columns.process_id} = x.{enp_processes.GetFullColumnName(enp_processes.columns.id)}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.process_id} IS NOT NULL
                                
                        END;
                        ");
                }
                {
                    //BOM Manufacturing Overhead Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_manufacturingOverhead.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.manufacturing_overhead_share_percent},
                                {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.name} AS {enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.name)},
                                {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.yearly_fee} AS {enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.yearly_fee)},
                                {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.id} AS {enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.id)},
                                {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.yearly_production_capacity} AS {enp_manufacturing_overhead.GetFullColumnName(enp_manufacturing_overhead.columns.yearly_production_capacity)}
                            FROM {enp_bom.TableName} LEFT JOIN {enp_manufacturing_overhead.TableName}
                            ON {enp_bom.TableName}.{enp_bom.columns.manufacturing_overhead_id} = {enp_manufacturing_overhead.TableName}.{enp_manufacturing_overhead.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.manufacturing_overhead_id} IS NOT NULL
                                
                        END;
                        ");
                }
                {
                    //BOM Mold Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_mold.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.mold_consumption_coefficient},
                                {enp_molds.TableName}.{enp_molds.columns.part_name} AS {enp_molds.GetFullColumnName(enp_molds.columns.part_name)},
                                {enp_molds.TableName}.{enp_molds.columns.mold_id} AS {enp_molds.GetFullColumnName(enp_molds.columns.mold_id)},
                                {enp_molds.TableName}.{enp_molds.columns.id} AS {enp_molds.GetFullColumnName(enp_molds.columns.id)},
                                {enp_molds.TableName}.{enp_molds.columns.tech_id} AS {enp_molds.GetFullColumnName(enp_molds.columns.tech_id)},
                                {enp_molds.TableName}.{enp_molds.columns.type} AS {enp_molds.GetFullColumnName(enp_molds.columns.type)},
                                {enp_molds.TableName}.{enp_molds.columns.life_time} AS {enp_molds.GetFullColumnName(enp_molds.columns.life_time)},
                                {enp_molds.TableName}.{enp_molds.columns.fastener_count} AS {enp_molds.GetFullColumnName(enp_molds.columns.fastener_count)},
                                {enp_molds.TableName}.{enp_molds.columns.design_fee} AS {enp_molds.GetFullColumnName(enp_molds.columns.design_fee)},
                                {enp_molds.TableName}.{enp_molds.columns.material_fee} AS {enp_molds.GetFullColumnName(enp_molds.columns.material_fee)},
                                {enp_molds.TableName}.{enp_molds.columns.machining_fee} AS {enp_molds.GetFullColumnName(enp_molds.columns.machining_fee)},
                                {enp_molds.TableName}.{enp_molds.columns.other_fee} AS {enp_molds.GetFullColumnName(enp_molds.columns.other_fee)}
                            FROM {enp_bom.TableName} LEFT JOIN {enp_molds.TableName}
                            ON {enp_bom.TableName}.{enp_bom.columns.mold_id} = {enp_molds.TableName}.{enp_molds.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.TableName}.{enp_bom.columns.mold_id} IS NOT NULL
                                
                        END;
                        ");
                }
                {
                    //BOM packing Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_packing.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.packing_consumption_coefficient},
                                x.*
                            FROM {enp_bom.TableName} LEFT JOIN
                                (SELECT {enp_packing.TableName}.{enp_packing.columns.name} AS {enp_packing.GetFullColumnName(enp_packing.columns.name)},
                                    {enp_packing.TableName}.{enp_packing.columns.price} AS {enp_packing.GetFullColumnName(enp_packing.columns.price)},
                                    {enp_packing.TableName}.{enp_packing.columns.id} AS {enp_packing.GetFullColumnName(enp_packing.columns.id)},
                                    {enp_units.TableName}.{enp_units.columns.name} AS {enp_units.GetFullColumnName(enp_units.columns.name)}
                                FROM {enp_packing.TableName} LEFT JOIN {enp_units.TableName}
                                ON {enp_packing.TableName}.{enp_packing.columns.unit_id} = {enp_units.TableName}.{enp_units.columns.id}) x
                            ON {enp_bom.TableName}.{enp_bom.columns.packing_id} = x.{enp_packing.GetFullColumnName(enp_packing.columns.id)}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.packing_id} IS NOT NULL
                                
                        END;
                        ");
                }
                {
                    //BOM Manufacturing Overhead Report
                    var procedureName = enp_bom.procedures.reports.procedure_enp_bom_report_transportation.name;
                    procedures.Add(procedureName, $@"
                        CREATE PROCEDURE {procedureName}
                        @{enp_bom.columns.product_id} BIGINT
                        AS
                        BEGIN
                            SELECT {enp_bom.TableName}.{enp_bom.columns.transportation_consumption_coefficient},
                                {enp_transportation.TableName}.{enp_transportation.columns.name} AS {enp_transportation.GetFullColumnName(enp_transportation.columns.name)},
                                {enp_transportation.TableName}.{enp_transportation.columns.price} AS {enp_transportation.GetFullColumnName(enp_transportation.columns.price)},
                                {enp_transportation.TableName}.{enp_transportation.columns.id} AS {enp_transportation.GetFullColumnName(enp_transportation.columns.id)}
                            FROM {enp_bom.TableName} LEFT JOIN {enp_transportation.TableName}
                            ON {enp_bom.TableName}.{enp_bom.columns.transportation_id} = {enp_transportation.TableName}.{enp_transportation.columns.id}
                            WHERE {enp_bom.columns.product_id} = @{enp_bom.columns.product_id} AND {enp_bom.columns.transportation_id} IS NOT NULL
                                
                        END;
                        ");
                }
                //Debug.WriteLine(string.Join("\nGO\n", procedures.Values));
                //Debug.WriteLine(sequences[sequenceName] + "\nGO\n" + string.Join("\nGO\n", tables.Values) + "\nGO\n" + string.Join("\nGO\n", procedures.Values));
            }
        }
       
    }
}
