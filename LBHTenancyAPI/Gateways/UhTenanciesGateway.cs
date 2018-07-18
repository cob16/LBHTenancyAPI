﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using LBHTenancyAPI.Domain;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Rewrite.Internal;
using Microsoft.EntityFrameworkCore.Internal;

namespace LBHTenancyAPI.Gateways
{
    public class UhTenanciesGateway : ITenanciesGateway
    {
        private readonly SqlConnection conn;

        public UhTenanciesGateway(string connectionString)
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
        }

        public List<TenancyListItem> GetTenanciesByRefs(List<string> tenancyRefs)
        {
            var all = conn.Query<TenancyListItem>(
                "SELECT " +
                "tenagree.tag_ref as TenancyRef, " +
                "tenagree.cur_bal as CurrentBalance, " +
                "contacts.con_name as PrimaryContactName, " +
                "contacts.con_address as PrimaryContactShortAddress, " +
                "contacts.con_postcode as PrimaryContactPostcode, " +
                "araction.tag_ref AS TenancyRef, " +
                "araction.action_code AS LastActionCode, " +
                "araction.action_date AS LastActionDate, " +
                "arag.arag_status as ArrearsAgreementStatus, " +
                "arag.arag_startdate as ArrearsAgreementStartDate " +
                "FROM tenagree " +
                "LEFT JOIN contacts " +
                "ON contacts.tag_ref = tenagree.tag_ref " +
                "LEFT JOIN ( " +
                "SELECT " +
                "araction.tag_ref, " +
                "araction.action_code, " +
                "araction.action_date " +
                "FROM araction " +
                $"WHERE araction.tag_ref IN ('{tenancyRefs.Join("', '")}') " +
                ") AS araction ON araction.tag_ref = tenagree.tag_ref " +
                "LEFT JOIN ( " +
                "SELECT " +
                "arag.tag_ref," +
                "arag.arag_status, " +
                "arag.arag_startdate " +
                "FROM arag " +
                $"WHERE arag.tag_ref IN ('{tenancyRefs.Join("', '")}') " +
                ") AS arag ON arag.tag_ref = tenagree.tag_ref " +
                $"WHERE tenagree.tag_ref IN ('{tenancyRefs.Join("', '")}') " +
                "ORDER BY arag.arag_startdate DESC, araction.action_date DESC"
            ).ToList();

            List<TenancyListItem> results = new List<TenancyListItem>();
            foreach (var reference in tenancyRefs)
            {
                results.Add(all.First(e => e.TenancyRef == reference));
            }

            return results;
        }

        public Tenancy GetTenancyForRef(string tenancyRef)
        {
            Tenancy result = conn.Query<Tenancy>($"" +
                                       $"SELECT DISTINCT" +
                                       $"(tenagree.tag_ref) as TenancyRef, " +
                                       $"tenagree.cur_bal as CurrentBalance, " +
                                       $"arag.arag_status as ArrearsAgreementStatus, " +
                                       $"arag.arag_startdate as ArrearsAgreementStartDate, " +
                                       $"contacts.con_name as PrimaryContactName, " +
                                       $"contacts.con_address as PrimaryContactLongAddress, " +
                                       $"contacts.con_postcode as PrimaryContactPostcode, " +
                                       $"contacts.con_phone1 as PrimaryContactPhone, " +
                                       $"araction.action_code as LastActionCode, " +
                                       $"araction.action_date as LastActionDate " +
                                       $"FROM tenagree " +
                                       $"LEFT JOIN arag " +
                                       $"ON arag.tag_ref = tenagree.tag_ref " +
                                       $"LEFT JOIN contacts " +
                                       $"ON contacts.tag_ref = tenagree.tag_ref " +
                                       $"LEFT JOIN araction " +
                                       $"ON araction.tag_ref = tenagree.tag_ref " +
                                       $"WHERE tenagree.tag_ref = ('{tenancyRef}') " +
                                       $"ORDER BY arag.arag_startdate DESC, araction.action_date DESC")
                .FirstOrDefault();

            result.ArrearsAgreements = GetLastFiveAgreementsForTenancy(tenancyRef);
            result.ArrearsActionDiary = GetLatestfiveArrearsActionForRef(tenancyRef);
            return result;
        }

        private List<ArrearsAgreementDetail> GetLastFiveAgreementsForTenancy(string tenancyRef)
        {
            return conn.Query<ArrearsAgreementDetail>("SELECT TOP 5" +
                                                      "tag_ref AS TenancyRef," +
                                                      "arag_status AS Status, " +
                                                      "arag_startdate Startdate, " +
                                                      "arag_amount Amount, " +
                                                      "arag_frequency AS Frequency, " +
                                                      "arag_breached AS Breached, " +
                                                      "arag_startbal AS StartBalance, " +
                                                      "arag_clearby AS ClearBy " +
                                                      "FROM arag " +
                                                      $"WHERE tag_ref = '{tenancyRef}'" +
                                                      $"ORDER BY arag_startdate DESC ").ToList();
        }

            public List<ArrearsActionDiaryDetails> GetLatestfiveArrearsActionForRef(string tenancyRef)
        {
            return conn.Query<ArrearsActionDiaryDetails>($"" +
                                                  $"SELECT top 5" +
                                                  $"tag_ref as TenancyRef, " +
                                                  $"action_code as ActionCode, " +
                                                  $"action_code_name as ActionCodeName, " +
                                                  $"action_date as ActionDate, " +
                                                  $"action_comment as ActionComment, " +
                                                  $"uh_username as UHUsername, " +
                                                  $"action_balance as ActionBalance " +
                                                  $"FROM araction " +
                                                  $"WHERE tag_ref = ('{tenancyRef}') " +
                                                  $"ORDER BY araction.action_date DESC").ToList();

        }

        public List<ArrearsActionDiaryDetails> GetActionDiaryDetailsbyTenancyRef(string tenancyRef)
        {
            return conn.Query<ArrearsActionDiaryDetails>($"" +
                                                         $"SELECT " +
                                                         $"tag_ref as TenancyRef, " +
                                                         $"action_code as ActionCode, " +
                                                         $"action_code_name as ActionCodeName, " +
                                                         $"action_date as ActionDate, " +
                                                         $"action_comment as ActionComment, " +
                                                         $"uh_username as UHUsername, " +
                                                         $"action_balance as ActionBalance " +
                                                         $"FROM araction " +
                                                         $"WHERE tag_ref = ('{tenancyRef}') " +
                                                         $"ORDER BY araction.action_date DESC").ToList();

        }


    }
}
