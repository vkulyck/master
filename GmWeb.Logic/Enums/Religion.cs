using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GmWeb.Logic.Enums;
public enum Religion
{
    [Display(Name = "Atheist")]
    Atheist,
    [Display(Name = "Christian/Eastern Orthodox")]
    ChristianEasternOrthodox,
    [Display(Name = "Christian/Catholic")]
    ChristianCatholic,
    [Display(Name = "Christian/Proto-Protestant")]
    ChristianProtoProtestant,
    [Display(Name = "Christian/Lutheran")]
    ChristianLutheran,
    [Display(Name = "Christian/Presbyterianism")]
    ChristianPresbyterianism,
    [Display(Name = "Christian/Anglican")]
    ChristianAnglican,
    [Display(Name = "Christian/Anabaptist")]
    ChristianAnabaptist,
    [Display(Name = "Christian/Methodist")]
    ChristianMethodist,
    [Display(Name = "Christian/Seventh day Adventist")]
    ChristianSeventhDay,
    [Display(Name = "Christian/Quaker")]
    ChristianQuaker,
    [Display(Name = "Christian/Plymouth Brethren")]
    ChristianPlymouthBrethren,
    [Display(Name = "Christian/Irvingist")]
    ChristianIrvingist,
    [Display(Name = "Christian/Pentecostal")]
    ChristianPentecostal,
    [Display(Name = "Christian/Evangelical")]
    ChristianEvangelical,
    [Display(Name = "Christian/Eastern Protestant")]
    ChristianEasternProtestant,
    [Display(Name = "Christian/Mormonism")]
    ChristianMormonism,
    [Display(Name = "Islam")]
    Islam,
    [Display(Name = "Buddhism")]
    Buddhism,
    [Display(Name = "Hinduism")]
    Hinduism,
    [Display(Name = "Judaism")]
    Judaism
}
