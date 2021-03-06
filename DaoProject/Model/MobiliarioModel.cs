﻿using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using DaoProject.Dao;
using DaoProject.DbAccess;
using ScjnUtilities;

namespace DaoProject.Model
{
    public class MobiliarioModel
    {
        private readonly Mobiliario mobiliario;

        public MobiliarioModel()
        {
        }

        public MobiliarioModel(Mobiliario mobiliario)
        {
            this.mobiliario = mobiliario;
        }

        /// <summary>
        /// Devuelve la lista de mobiliario de acuerdo al parametro solicitado
        /// </summary>
        /// <param name="parametro">Columna de la base de datos contra la cual se buscara</param>
        /// <param name="valor">Valor Buscado</param>
        /// <returns></returns>
        public ObservableCollection<Mobiliario> GetMobiliarioPorParametro(String parametro, String valor)
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataReader dataReader;
            ObservableCollection<Mobiliario> listaMobiliario = new ObservableCollection<Mobiliario>();

            int number = 0;
            int.TryParse(valor.ToString(), out number);

            try
            {
                connection.Open();

                string selstr = "SELECT M.*,T.Descripcion FROM Mobiliario M INNER JOIN TiposEquipos T ON T.idTipo = M.idTipo WHERE " +
                                parametro + " = @Parametro AND T.idInventario = 2";
                SqlCommand cmd = new SqlCommand(selstr, connection);
                SqlParameter param = cmd.Parameters.Add("@Parametro", (number > 0) ? SqlDbType.Int : SqlDbType.NVarChar, 0);
                param.Value = valor;

                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        Mobiliario mobiliarioL = new Mobiliario();
                        mobiliarioL.IdMobiliario = Convert.ToInt32(dataReader["idMobiliario"]);
                        mobiliarioL.IdTipoMobiliario = Convert.ToInt32(dataReader["idTipo"]);
                        mobiliarioL.TipoMobiliario = dataReader["Descripcion"].ToString();
                        mobiliarioL.Inventario = Convert.ToInt32(dataReader["NoInventario"]);
                        mobiliarioL.Expediente = Convert.ToInt32(dataReader["Expediente"]);
                        mobiliarioL.Observaciones = dataReader["Observaciones"].ToString();
                        mobiliarioL.FechaAlta = DateTimeUtilities.GetDateFromReader(dataReader, "FechaAlta");
                        mobiliarioL.FechaModificacion = DateTimeUtilities.GetDateFromReader(dataReader, "FechaModificacion");

                        listaMobiliario.Add(mobiliarioL);
                    }
                }
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
            return listaMobiliario;
        }

        /// <summary>
        /// Agrega un nuevo bien (mobiliario) al catálogo de la coordinación
        /// </summary>
        public void SetNewMobiliario()
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter dataAdapter;

            try
            {
                DataSet dataSet = new DataSet();
                DataRow dr;

                string sqlCadena = "SELECT * FROM Mobiliario WHERE expediente = 0";

                dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCadena, connection);

                dataAdapter.Fill(dataSet, "Mobiliario");

                dr = dataSet.Tables["Mobiliario"].NewRow();
                dr["idTipo"] = mobiliario.IdTipoMobiliario;
                dr["NoInventario"] = mobiliario.Inventario;
                dr["Expediente"] = mobiliario.Expediente;
                dr["Observaciones"] = mobiliario.Observaciones;

                dataSet.Tables["Mobiliario"].Rows.Add(dr);

                dataAdapter.InsertCommand = connection.CreateCommand();
                dataAdapter.InsertCommand.CommandText = "INSERT INTO Mobiliario(idTipo,NoInventario,Expediente,Observaciones,FechaAlta,FechaModificacion)" +
                                                        " VALUES(@idTipo,@NoInventario,@Expediente,@Observaciones,SysDateTime(),SysDateTime())";

                dataAdapter.InsertCommand.Parameters.Add("@idTipo", SqlDbType.Int, 0, "idTipo");
                dataAdapter.InsertCommand.Parameters.Add("@NoInventario", SqlDbType.NVarChar, 0, "NoInventario");
                dataAdapter.InsertCommand.Parameters.Add("@Expediente", SqlDbType.Int, 0, "Expediente");
                dataAdapter.InsertCommand.Parameters.Add("@Observaciones", SqlDbType.Text, 0, "Observaciones");

                dataAdapter.Update(dataSet, "Mobiliario");

                dataSet.Dispose();
                dataAdapter.Dispose();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Actualiza los datos del mobiliario en cuestión
        /// </summary>
        public void UpdateMobiliario()
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            DataSet dataSet = new DataSet();
            DataRow dr;

            string sqlCadena = "SELECT * FROM Mobiliario WHERE idMobiliario = " + mobiliario.IdMobiliario;

            try
            {
                dataAdapter.SelectCommand = new SqlCommand(sqlCadena, connection);
                dataAdapter.Fill(dataSet, "Mobiliario");

                dr = dataSet.Tables["Mobiliario"].Rows[0];
                dr.BeginEdit();
                dr["idTipo"] = mobiliario.IdTipoMobiliario;
                dr["NoInventario"] = mobiliario.Inventario;
                dr["Observaciones"] = mobiliario.Observaciones;
                dr["FechaModificacion"] = DateTime.Now.ToString("yyyy/MM/dd");
                dr.EndEdit();

                dataAdapter.UpdateCommand = connection.CreateCommand();
                dataAdapter.UpdateCommand.CommandText = "UPDATE Mobiliario SET idTipo = @idTipo, NoInventario = @NoInventario," +
                                                        "Observaciones = @Observaciones, FechaModificacion = @FechaModificacion " +
                                                        "WHERE idMobiliario = @idMobiliario";

                dataAdapter.UpdateCommand.Parameters.Add("@idTipo", SqlDbType.Int, 0, "idTipo");
                dataAdapter.UpdateCommand.Parameters.Add("@NoInventario", SqlDbType.VarChar, 0, "NoInventario");
                dataAdapter.UpdateCommand.Parameters.Add("@Observaciones", SqlDbType.Text, 0, "Observaciones");
                dataAdapter.UpdateCommand.Parameters.Add("@FechaModificacion", SqlDbType.Date, 0, "FechaModificacion");
                dataAdapter.UpdateCommand.Parameters.Add("@idMobiliario", SqlDbType.Int, 0, "idMobiliario");

                dataAdapter.Update(dataSet, "Mobiliario");

                dataSet.Dispose();
                dataAdapter.Dispose();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Actualiza el número de expediente del usuario que tiene el resguardo del mobiliario
        /// </summary>
        /// <param name="historial"></param>
        public void UpdateMobiliario(HistorialMobiliario historial)
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            DataSet dataSet = new DataSet();
            DataRow dr;

            string sqlCadena = "SELECT * FROM Mobiliario WHERE idMobiliario = " + mobiliario.IdMobiliario;

            try
            {
                dataAdapter.SelectCommand = new SqlCommand(sqlCadena, connection);
                dataAdapter.Fill(dataSet, "Mobiliario");

                dr = dataSet.Tables["Mobiliario"].Rows[0];
                dr.BeginEdit();
                dr["Expediente"] = historial.ExpActual;
                dr["Observaciones"] = historial.Observaciones;
                dr.EndEdit();

                //dataSet.Tables["Mobiliario"].Rows.Add(dr);

                dataAdapter.UpdateCommand = connection.CreateCommand();
                dataAdapter.UpdateCommand.CommandText = "UPDATE Mobiliario SET Expediente = @Expediente,Observaciones = @Observaciones,FechaModificacion = SysDateTime() " +
                                                        "WHERE idMobiliario = @idMobiliario";

                dataAdapter.UpdateCommand.Parameters.Add("@Expediente", SqlDbType.Int, 0, "Expediente");
                dataAdapter.UpdateCommand.Parameters.Add("@Observaciones", SqlDbType.Text, 0, "Observaciones");
                dataAdapter.UpdateCommand.Parameters.Add("@idMobiliario", SqlDbType.Int, 0, "idMobiliario");

                dataAdapter.Update(dataSet, "Mobiliario");

                dataSet.Dispose();
                dataAdapter.Dispose();

                this.InsertaHistorialMobiliario(historial);
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Inserta el registro del movimiento de reasignacion de mobiliario  dentro de la tabla historial
        /// </summary>
        /// <param name="servidorActual"></param>
        /// <param name="servidorNuevo"></param>
        private void InsertaHistorialMobiliario(HistorialMobiliario historial)
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter dataAdapter;

            DataSet dataSet = new DataSet();
            DataRow dr;

            string sqlCadena = "SELECT * FROM HistorialMobiliario WHERE idMovimiento = 0";

            try
            {
                dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(sqlCadena, connection);

                dataAdapter.Fill(dataSet, "Mobiliario");

                dr = dataSet.Tables["Mobiliario"].NewRow();
                dr["idMobiliario"] = historial.IdMobiliario;
                dr["expAnterior"] = historial.ExpAnterior;
                dr["expActual"] = historial.ExpActual;
                dr["Observaciones"] = historial.Observaciones;
                dr["usuarioModifica"] = AccesoUsuarioModel.Usuario;

                dataSet.Tables["Mobiliario"].Rows.Add(dr);

                dataAdapter.InsertCommand = connection.CreateCommand();
                dataAdapter.InsertCommand.CommandText = "INSERT INTO HistorialMobiliario(idMobiliario,expAnterior,expActual,Observaciones,FechaReasignacion,usuarioModifica)" +
                                                        " VALUES(@idMobiliario,@expAnterior,@expActual,@Observaciones,SysDateTime(),@usuarioModifica)";

                dataAdapter.InsertCommand.Parameters.Add("@idMobiliario", SqlDbType.Int, 0, "idMobiliario");
                dataAdapter.InsertCommand.Parameters.Add("@expAnterior", SqlDbType.Int, 0, "expAnterior");
                dataAdapter.InsertCommand.Parameters.Add("@expActual", SqlDbType.Int, 0, "expActual");
                dataAdapter.InsertCommand.Parameters.Add("@Observaciones", SqlDbType.Text, 0, "Observaciones");
                dataAdapter.InsertCommand.Parameters.Add("@usuarioModifica", SqlDbType.VarChar, 0, "usuarioModifica");

                dataAdapter.Update(dataSet, "Mobiliario");

                dataSet.Dispose();
                dataAdapter.Dispose();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }

        public void BajaMobiliario(String observaciones)
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlCommand cmd;

            cmd = connection.CreateCommand();
            cmd.Connection = connection;

            try
            {
                connection.Open();

                this.ActualizaObservacionesMobiliarioBaja(mobiliario, observaciones);
                cmd.CommandText = "DELETE FROM Mobiliario WHERE NoInventario = '" + mobiliario.Inventario + "'";
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }
        
        private void ActualizaObservacionesMobiliarioBaja(Mobiliario mobiliario, String observaciones)
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();

            DataSet dataSet = new DataSet();
            DataRow dr;

            string sqlCadena = "SELECT * FROM Mobiliario WHERE NoInventario = '" + mobiliario.Inventario + "'";

            try
            {
                dataAdapter.SelectCommand = new SqlCommand(sqlCadena, connection);
                dataAdapter.Fill(dataSet, "Mobiliario");

                dr = dataSet.Tables["Mobiliario"].Rows[0];
                dr.BeginEdit();
                dr["Observaciones"] = observaciones;
                dr.EndEdit();

                dataAdapter.UpdateCommand = connection.CreateCommand();
                dataAdapter.UpdateCommand.CommandText = "UPDATE Mobiliario SET  Observaciones = @Observaciones " +
                                                        "WHERE NoInventario = @NoInventario ";

                dataAdapter.UpdateCommand.Parameters.Add("@Observaciones", SqlDbType.VarChar, 0, "Observaciones");
                dataAdapter.UpdateCommand.Parameters.Add("@NoInventario", SqlDbType.VarChar, 0, "NoInventario");

                dataAdapter.Update(dataSet, "Mobiliario");

                dataSet.Dispose();
                dataAdapter.Dispose();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }

        public DataSet GetBajas()
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter vAdap;
            DataSet vDs = null;


            try
            {
                connection.Open();
                string vQuery = "SELECT * FROM vMBajas ORDER By FechaBaja Desc";

                vDs = new DataSet();
                vAdap = new SqlDataAdapter(vQuery, connection);
                vAdap.Fill(vDs, "vMBajas");
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
            return vDs;
        }


        #region Historial

        /// <summary>
        /// Devuelve el listado de movimientos que ha registrado un equipo
        /// </summary>
        /// <param name="mobiliario"></param>
        /// <returns></returns>
        public ObservableCollection<HistorialMobiliario> GetHistorial(Mobiliario mobiliario)
        {
            ObservableCollection<HistorialMobiliario> historiales = new ObservableCollection<HistorialMobiliario>();

            SqlConnection connection = Conexion.GetConexion();
            SqlDataReader dataReader;

            try
            {
                connection.Open();

                string selstr = "SELECT * FROM HistorialMobiliario  WHERE IdMobiliario = @IdMobiliario ORDER BY fechaReasignacion ";
                
                SqlCommand cmd = new SqlCommand(selstr, connection);
                cmd.Parameters.AddWithValue("@IdMobiliario", mobiliario.Inventario);
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        HistorialMobiliario historial = new HistorialMobiliario();
                        historial.IdMovimiento = Convert.ToInt32(dataReader["IdMovimiento"]);
                        historial.ExpAnterior = Convert.ToInt32(dataReader["ExpAnterior"]);
                        historial.ExpActual = Convert.ToInt32(dataReader["ExpActual"]);
                        historial.Observaciones = dataReader["Observaciones"].ToString();
                        historial.FechaReasignacion = DateTimeUtilities.GetDateFromReader(dataReader, "FechaReasignacion"); 

                        historiales.Add(historial);
                    }
                }
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
            return historiales;
        }


        public DataSet GetHistorial()
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataAdapter vAdap;
            DataSet vDs = null;


            try
            {
                connection.Open();
                string vQuery = "SELECT * FROM HMobiliario";

                vDs = new DataSet();
                vAdap = new SqlDataAdapter(vQuery, connection);
                vAdap.Fill(vDs, "HMobiliario");
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,MobiliarioModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
            return vDs;
        }

        #endregion
    }
}
