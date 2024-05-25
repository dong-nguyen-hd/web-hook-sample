﻿namespace WebHookSample.Resources.Enums;

public enum ProcessType : byte
{
    Fail = 0,
    Success = 1,
    Init = 2,
    Timeout = 3,
    Retry = 4,
}
