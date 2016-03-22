using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPCManager.Domain.Constants
{
    /// <summary>
    /// 각종 기능에서 사용하는 작동 옵션을 정의한다. 예를 들어
    /// - 기본 조회(View) 옵션
    /// - 복사(Copy) 옵션
    /// - 개정(Revise) 옵션
    /// - 일정 계산 옵션
    /// </summary>
    [Flags]
    public enum OperationOptions
    {   
        None             = 0x0,
        MainAttribute    = 0x1,
        LinkAttribute    = 0x2,
        UDA              = 0x4,
        RelatedItem      = 0x8,
        File             = 0x10,
        ReferenceProject = 0x20,
        BOM              = 0x40,
        BOMFullLevel     = 0x80,
        AffectedObject   = 0x100,
        WBS              = 0x200,
        WBSFullLevel     = 0x400,
        Resource         = 0x800,
        Forward          = 0x1000,
        Backward         = 0x2000
    }
}
