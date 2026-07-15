namespace JaPe.ServiceManager.Enums;

internal static class NativeConstants
{
   public const uint ScManagerCreateService = 0x0002;
   public const uint ScManagerEnumerateService = 0x0004;
   public const uint ServiceAllAccess = 0xF01FF;
   
   public const int ServiceQueryStatus = 0x0004;
   public const int ServiceStart = 0x0010;
   public const int ServiceStop = 0x0020;
   public const int ReadControl = 0x00020000;

   public const int ServiceStartStopAccess = 
      ReadControl | 
      ServiceQueryStatus |
      ServiceStart | 
      ServiceStop;
}