﻿using System;
using System.Data.SqlClient;
using System.Linq;
using DaoProject.Dao;
using DaoProject.DbAccess;
using ScjnUtilities;
using System.Collections.ObjectModel;

namespace DaoProject.Model
{
    public class LevantaReporteModel
    {
        /// <summary>
        /// Ingresa un nuevo registro con la información del reporte que esta siendo levantado
        /// </summary>
        /// <param name="reporte"></param>
        public void SetNewReporte(LevantaReporte reporte)
        {
            SqlConnection connection = Conexion.GetConexion();

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO ReportesComputo(FechaReporte,FechaReporteInt,IdEquipo,Expediente,Reporto,Problema,NumReporte)" +
                                                " VALUES(@FechaReporte,@FechaReporteInt,@IdEquipo,@Expediente,@Reporto,@Problema,@NumReporte)", connection);
                cmd.Parameters.AddWithValue("@FechaReporte", reporte.FechaReporte);
                cmd.Parameters.AddWithValue("@FechaReporteInt", DateTimeUtilities.DateToInt(reporte.FechaReporte));
                cmd.Parameters.AddWithValue("@IdEquipo", reporte.IdEquipo);
                cmd.Parameters.AddWithValue("@Expediente", reporte.Expediente);
                cmd.Parameters.AddWithValue("@Reporto", reporte.Reporto);
                cmd.Parameters.AddWithValue("@Problema", reporte.Problema);
                cmd.Parameters.AddWithValue("@NumReporte", reporte.NumReporte);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,LevantaReporteModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,LevantaReporteModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Actualiza los datos de cierre de reporte
        /// </summary>
        /// <param name="reporte"></param>
        public void UpdateReporte(LevantaReporte reporte)
        {
            SqlConnection connection = Conexion.GetConexion();

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE ReportesComputo SET Problema = @Problema, Atendio = @Atendio," +
                                                "FechaCierre = @FechaCierre, FechaCierreInt = @FechaCierreInt, Observaciones = @Observaciones " +
                                                " WHERE IdReporte = @IdReporte", connection);
                cmd.Parameters.AddWithValue("@Problema", reporte.Problema);
                cmd.Parameters.AddWithValue("@Atendio", reporte.Atendio);
                cmd.Parameters.AddWithValue("@FechaCierre", reporte.FechaCierre);
                cmd.Parameters.AddWithValue("@FechaCierreInt", DateTimeUtilities.DateToInt(reporte.FechaCierre));
                cmd.Parameters.AddWithValue("@Observaciones", reporte.Observaciones);
                cmd.Parameters.AddWithValue("@IdReporte", reporte.IdReporte);
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,LevantaReporteModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,LevantaReporteModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }
        }


        public ObservableCollection<LevantaReporte> GetReportes()
        {
            SqlConnection connection = Conexion.GetConexion();
            SqlDataReader reader;

            ObservableCollection<LevantaReporte> listaReportes = new ObservableCollection<LevantaReporte>();

            try
            {
                connection.Open();

                string selstr = "select R.*,U.Nombre,E.SC_Equipo,T.Descripcion, " +
                    "(SELECT nombre from USuarios WHERE Expediente = R.Reporto) Reporto2 " +
                    " FROM ReportesComputo R " + 
                    " INNER JOIN Usuarios U ON U.Expediente = R.Expediente " + 
                    " INNER JOIN Equipos E ON E.IdEquipo = R.IdEquipo " +
                    " INNER JOIN TiposEquipos T ON E.IdTipo = T.IdTipo " + 
                    " WHERE T.IdInventario = 1";

                SqlCommand cmd = new SqlCommand(selstr, connection);
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        LevantaReporte reporte = new LevantaReporte();
                        reporte.IdReporte = Convert.ToInt32(reader["IdReporte"]);
                        reporte.FechaReporte = DateTimeUtilities.GetDateFromReader(reader, "FechaReporte");
                        reporte.FechaReporteInt = Convert.ToInt32(reader["FechaReporteInt"]);
                        reporte.IdEquipo = Convert.ToInt32(reader["idEquipo"]);
                        reporte.Expediente = Convert.ToInt32(reader["Expediente"]);
                        reporte.Nombre = reader["Nombre"].ToString();
                        reporte.Reporto = Convert.ToInt32(reader["Reporto"]);
                        reporte.Atendio = reader["Atendio"].ToString();
                        reporte.FechaCierre = DateTimeUtilities.GetDateFromReader(reader, "FechaCierre");
                        reporte.Observaciones = DataBaseUtilities.VerifyDbNullForStrings(reader, "Observaciones");
                        reporte.NumReporte = Convert.ToInt32(reader["NumReporte"]);
                        reporte.ScEquipo = reader["SC_Equipo"].ToString();
                        reporte.TipoEquipo = reader["Descripcion"].ToString();
                        reporte.ReportoStr = reader["Reporto2"].ToString();
                        reporte.Problema = reader["Problema"].ToString();

                        listaReportes.Add(reporte);
                    }
                }

                reader.Close();
                selstr = null;
            }
            catch (SqlException ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,LevantaReporteModel", "Inventario");
            }
            catch (Exception ex)
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                ErrorUtilities.SetNewErrorMessage(ex, methodName + " Exception,LevantaReporteModel", "Inventario");
            }
            finally
            {
                connection.Close();
            }

            return listaReportes;
        }


    }
}