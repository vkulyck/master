using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum Medical
{
    [Display(Name = "State")]
    State,
    [Display(Name = "Insurance Provider")]
    Insurance,
    [Display(Name = "Member Number")]
    Member,
    [Display(Name = "Coverage renewal date")]
    Coverage,
    [Display(Name = "Plan/Plan Name")]
    PlanName,
    [Display(Name = "Member ID Number")]
    MemberID,
    [Display(Name = "Group ID")]
    GroupID,
    [Display(Name = "Primary Care Provier")]
    PrimaryCare
}

public enum Medicare
{
    [Display(Name = "Medicare Part A")]
    PartA,
    [Display(Name = "Medicare Part B")]
    PartB,
    [Display(Name = "Medicare Parts A & B")]
    PartAB,
    [Display(Name = "Medicare Advantage plan / Medicare Part C / MA Plan")]
    MAPlan,
    [Display(Name = "Medicare Advantage plan / Medicare Part C / MA Plan WITH Part D")]
    MAPlanD,
    [Display(Name = "Medicare Part D")]
    PartD,
    [Display(Name = "Medicare Supplement (Medigap)")]
    Medigap
}