using System.Diagnostics;
using System.Data.SQLite;

namespace TechnicSolderHelper.SQL
{

    public class SolderSqlHandler
    {
        private string sqlLitePath;

        public SolderSqlHandler(string sqllitePath)
        {
            this.sqlLitePath = sqllitePath;
        }

   
        public void UpdateModversionMd5(string modslug, string modversion, string md5)
        {
            int id = GetModId(modslug);
            String sql = string.Format("UPDATE {0} SET md5=@md5 , updated_at=@update WHERE version LIKE @modversion AND mod_id LIKE @modid;", "modversions");
            using (SQLiteConnection conn = new(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@md5", md5);
                    cmd.Parameters.AddWithValue("@modversion", modversion);
                    cmd.Parameters.AddWithValue("@modid", id);
                    cmd.Parameters.AddWithValue("@update", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
         
        public int GetModpackId(string modpackName)
        {
            string sql = string.Format("SELECT id FROM {0} WHERE slug LIKE @modpack OR name LIKE @modpack",  "modpacks");
            using (SQLiteConnection conn = new(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@modpack", modpackName);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader["id"]);
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Adds a mod to solder.
        /// </summary>
        /// <param name="modslug">The slug. Cannot be null</param>
        /// <param name="description">The description of the mod. Can be null</param>
        /// <param name="author">The name of the mod author. Can be null</param>
        /// <param name="link">The link to the mod. Can be null</param>
        /// <param name="name">The pretty name of the mod. Cannot be null</param>
        public void AddModToSolder(string modslug, string description, string author, string link, string name)
        {
            string sql = string.Format("INSERT INTO {0}(name, description, author, link, pretty_name, created_at, updated_at) VALUES(@modslug, @descriptionValue, @authorValue, @linkValue, @name, @create, @update);", "mods");

            using (SQLiteConnection conn = new(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@modslug", modslug);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@create", DateTime.Now);
                    cmd.Parameters.AddWithValue("@update", DateTime.Now);
                    cmd.Parameters.AddWithValue("@descriptionValue",
                        string.IsNullOrWhiteSpace(description) ? "" : description);
                    cmd.Parameters.AddWithValue("@authorValue", string.IsNullOrWhiteSpace(author) ? "" : author);
                    cmd.Parameters.AddWithValue("@linkValue", string.IsNullOrWhiteSpace(link) ? "" : link);
                    if (GetModId(modslug) == -1)
                        cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Get the id of a mod on solder.
        /// </summary>
        /// <param name="slug">The modslug of the mod. Also known as the slug</param>
        /// <returns>Returns the modslug of the mod if found, otherwise returns -1.</returns>
        public int GetModId(String slug)
        {
            string sql = string.Format("SELECT id FROM {0} WHERE name LIKE @modname", "mods");
            using (SQLiteConnection conn = new(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@modname", slug);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader["id"]);
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Checks if a certain mod version is already on Solder.
        /// </summary>
        /// <param name="modid">The modslug</param>
        /// <param name="version">The version</param>
        /// <returns>Returns true if the mod version is on solder, false if not. </returns>
        private bool IsModversionOnline(int modid, string version)
        {
            string sql = string.Format("SELECT id FROM {0} WHERE version LIKE @version AND mod_id LIKE @modslug;", "modversions");
            using (SQLiteConnection conn = new(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@modslug", modid);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a certain mod version is already on Solder.
        /// </summary>
        /// <param name="modid">The modslug</param>
        /// <param name="version">The version</param>
        /// <returns>Returns true if the mod version is on solder, false if not. </returns>
        public bool IsModversionOnline(String modid, String version)
        {
            int id = GetModId(modid);
            if (id == -1) return false;
            return IsModversionOnline(id, version);
        }

        /// <summary>
        /// Adds a new mod version to Solder.
        /// </summary>
        /// <param name="modid">The modslug</param>
        /// <param name="version">The mod version</param>
        /// <param name="md5">The MD5 value of the zip</param>
        public void AddNewModversionToSolder(int modid, string version, string md5)
        {
            if (IsModversionOnline(modid, version))
                return;
            String sql = string.Format("INSERT INTO {0} (mod_id, version, md5, created_at, updated_at) VALUES(@modslug, @version, @md5, @create, @update);",  "modversions");
            using (SQLiteConnection conn = new(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@modslug", modid);
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@md5", md5);
                    cmd.Parameters.AddWithValue("@create", DateTime.Now);
                    cmd.Parameters.AddWithValue("@update", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Adds a new mod version to Solder.
        /// </summary>
        /// <param name="modid">The modslug</param>
        /// <param name="version">The mod version</param>
        /// <param name="md5">The MD5 value of the zip</param>
        public void AddNewModversionToSolder(string modid, string version, string md5)
        {
            int id = GetModId(modid);
            AddNewModversionToSolder(id, version, md5);
        }

        public void CreateNewModpack(String modpackName, String modpackSlug)
        {
            String sql = string.Format("INSERT INTO {0}(name, slug, created_at, updated_at, icon_md5, logo_md5, background_md5, recommended, latest, `order`, hidden, private, icon, logo, background) VALUES(@name, @slug, @create, @update, \"\",\"\",\"\",\"\",\"\", 0,1,0,0,0,0);", "modpacks");
            using (SQLiteConnection conn = new SQLiteConnection(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", modpackName);
                    cmd.Parameters.AddWithValue("@slug", modpackSlug);
                    cmd.Parameters.AddWithValue("@create", DateTime.Now);
                    cmd.Parameters.AddWithValue("@update", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CreateModpackBuild(int modpackId, String version, String mcVersion, String javaVersion, int memory)
        {
            String sql = string.Format("INSERT INTO {0}.{1}(modpack_id, version, minecraft, is_published, private, created_at, updated_at, min_java, min_memory) VALUES(@modpack, @version, @mcVersion, 0, 0, @create, @update, @minJava, @minMemory);",  "builds");
            using (SQLiteConnection conn = new SQLiteConnection(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@modpack", modpackId);
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@mcVersion", mcVersion);
                    cmd.Parameters.AddWithValue("@create", DateTime.Now);
                    cmd.Parameters.AddWithValue("@update", DateTime.Now);
                    cmd.Parameters.AddWithValue("@minJava", String.IsNullOrWhiteSpace(javaVersion) ? "" : javaVersion);
                    cmd.Parameters.AddWithValue("@minMemory", memory);
                    cmd.ExecuteNonQuery();
                }
                sql = string.Format("UPDATE {0} SET updated_at=@update WHERE id LIKE @id;",  "modpacks");
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@update", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id", modpackId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int GetBuildId(int modpackId, String version)
        {
            String sql = string.Format("SELECT id FROM {0}.{1} WHERE modpack_id LIKE @modpack AND version LIKE @version;", "builds");
            using (SQLiteConnection conn = new SQLiteConnection(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@modpack", modpackId);
                    cmd.Parameters.AddWithValue("@version", version);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader["id"].ToString());
                        }
                    }
                }
            }
            return -1;
        }

        public int GetModversionId(int modId, String version)
        {
            String sql = string.Format("SELECT id FROM {0} WHERE mod_id LIKE @mod AND version LIKE @version",  "modversions");
            using (SQLiteConnection conn = new SQLiteConnection(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@mod", modId);
                    cmd.Parameters.AddWithValue("@version", version);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return Convert.ToInt32(reader["id"].ToString());
                        }
                    }
                }
            }

            return -1;
        }

        private bool IsModversionInBuild(int build, int modversionId)
        {
            String sql = string.Format("SELECT id FROM {0} WHERE modversion_id LIKE @version AND build_id LIKE @build;", "build_modversion");
            using (SQLiteConnection conn = new SQLiteConnection(sqlLitePath))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@version", modversionId);
                    cmd.Parameters.AddWithValue("@build", build);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void AddModversionToBuild(int build, int modversionId)
        {
            if (!IsModversionInBuild(build, modversionId))
            {
                string sql = string.Format("INSERT INTO {0}(modversion_id, build_id, created_at, updated_at) VALUES(@modslug, @buildid, @created, @updated);",  "build_modversion");
                using (SQLiteConnection conn = new SQLiteConnection(sqlLitePath))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@modslug", modversionId);
                        cmd.Parameters.AddWithValue("@buildid", build);
                        cmd.Parameters.AddWithValue("@created", DateTime.Now);
                        cmd.Parameters.AddWithValue("@updated", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    sql = string.Format("UPDATE {0} SET updated_at=@update WHERE id LIKE @id;",  "builds");
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", build);
                        cmd.Parameters.AddWithValue("@update", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                    int modpackid;
                    sql = string.Format("SELECT modpack_id FROM {0} WHERE id LIKE @buildid;",  "builds");
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@buildid", build);
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();

                            modpackid = Convert.ToInt32(reader["modpack_id"].ToString());

                        }
                    }
                    sql = string.Format("UPDATE {0} SET updated_at=@update WHERE id LIKE @modpackid;", "modpacks");
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@update", DateTime.Now);
                        cmd.Parameters.AddWithValue("@modpackid", modpackid);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }
    }
}
