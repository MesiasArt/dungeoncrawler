import ctypes as c, ctypes.wintypes as w, sys, os, json, time
from pathlib import Path
K=c.WinDLL('kernel32',use_last_error=True); D=c.WinDLL('dbghelp',use_last_error=True)
U64=c.c_ulonglong; P=c.c_void_p; DWORD=w.DWORD
class SI(c.Structure):
 _fields_=[('cb',DWORD),('reserved',w.LPWSTR),('desktop',w.LPWSTR),('title',w.LPWSTR),('x',DWORD),('y',DWORD),('xs',DWORD),('ys',DWORD),('xc',DWORD),('yc',DWORD),('fill',DWORD),('flags',DWORD),('show',w.WORD),('cb2',w.WORD),('res2',P),('stdin',P),('stdout',P),('stderr',P)]
class PI(c.Structure): _fields_=[('process',P),('thread',P),('pid',DWORD),('tid',DWORD)]
class ER(c.Structure): _fields_=[('code',DWORD),('flags',DWORD),('record',P),('address',P),('count',DWORD),('info',U64*15)]
class EI(c.Structure): _fields_=[('record',ER),('first',DWORD)]
class CP(c.Structure): _fields_=[('file',P),('process',P),('thread',P),('base',P),('debugoffset',DWORD),('debugsize',DWORD),('tls',P),('start',P),('name',P),('unicode',w.WORD)]
class LD(c.Structure): _fields_=[('file',P),('base',P),('debugoffset',DWORD),('debugsize',DWORD),('name',P),('unicode',w.WORD)]
class EVU(c.Union): _fields_=[('exception',EI),('create',CP),('dll',LD),('exit',DWORD),('raw',c.c_byte*176)]
class EV(c.Structure): _fields_=[('code',DWORD),('pid',DWORD),('tid',DWORD),('u',EVU)]
class ADDR(c.Structure): _fields_=[('offset',U64),('segment',w.WORD),('mode',DWORD)]
class SYM(c.Structure):
 _fields_=[('size',DWORD),('index',DWORD),('reserved',U64*2),('index2',DWORD),('size2',DWORD),('base',U64),('flags',DWORD),('value',U64),('address',U64),('register',DWORD),('scope',DWORD),('tag',DWORD),('namelen',DWORD),('maxnamelen',DWORD),('name',c.c_char*1024)]
K.CreateProcessW.argtypes=[w.LPCWSTR,w.LPWSTR,P,P,w.BOOL,DWORD,P,w.LPCWSTR,c.POINTER(SI),c.POINTER(PI)];K.CreateProcessW.restype=w.BOOL
K.WaitForDebugEvent.argtypes=[c.POINTER(EV),DWORD];K.ContinueDebugEvent.argtypes=[DWORD,DWORD,DWORD]
K.OpenThread.argtypes=[DWORD,w.BOOL,DWORD];K.OpenThread.restype=P
K.GetThreadContext.argtypes=[P,P];K.CloseHandle.argtypes=[P]
K.GetFinalPathNameByHandleW.argtypes=[P,w.LPWSTR,DWORD,DWORD]
D.SymInitializeW.argtypes=[P,w.LPCWSTR,w.BOOL];D.SymInitializeW.restype=w.BOOL
D.SymLoadModuleExW.argtypes=[P,P,w.LPCWSTR,w.LPCWSTR,U64,DWORD,P,DWORD];D.SymLoadModuleExW.restype=U64
D.SymFromAddr.argtypes=[P,U64,c.POINTER(U64),P];D.SymFromAddr.restype=w.BOOL
D.StackWalk64.argtypes=[DWORD,P,P,P,P,P,P,P,P];D.StackWalk64.restype=w.BOOL
D.MiniDumpWriteDump.argtypes=[P,DWORD,P,DWORD,P,P,P];D.MiniDumpWriteDump.restype=w.BOOL
K.CreateFileW.argtypes=[w.LPCWSTR,DWORD,DWORD,P,DWORD,DWORD,P];K.CreateFileW.restype=P
exe,folder=sys.argv[1:3];out=Path(folder);out.mkdir(parents=True,exist_ok=True)
symbolpath=r'C:\Program Files\Unity\Hub\Editor\6000.5.4f1\Editor\Data\PlaybackEngines\windowsstandalonesupport\Variations\win64_player_nondevelopment_mono'
si=SI();si.cb=c.sizeof(si);pi=PI();cmd=c.create_unicode_buffer('"'+exe+'" -screen-width 1280 -screen-height 720 -screen-fullscreen 0 -logFile "'+str(out/'Player.log')+'" '+ ' '.join(sys.argv[3:]))
if not K.CreateProcessW(exe,cmd,None,None,False,2,None,str(Path(exe).parent),c.byref(si),c.byref(pi)):raise c.WinError(c.get_last_error())
D.SymSetOptions(0x16);D.SymInitializeW(pi.process,symbolpath,False)
(out/'pid.txt').write_text(str(pi.pid)); modules=[]; faults=[];started=time.time()
def modulepath(handle):
 b=c.create_unicode_buffer(32768);n=K.GetFinalPathNameByHandleW(handle,b,len(b),0);return b.value if n else ''
def symbolize(addr):
 s=SYM();s.size=88;s.maxnamelen=1024;disp=U64(); ok=D.SymFromAddr(pi.process,addr,c.byref(disp),c.byref(s));return str(hex(addr))+' '+(s.name[:s.namelen].decode(errors='replace')+'+'+hex(disp.value) if ok else next((m['path']+'+'+hex(addr-m['base']) for m in sorted(modules,key=lambda m:m['base'],reverse=True) if addr>=m['base']),'?'))
while True:
 e=EV()
 if not K.WaitForDebugEvent(c.byref(e),1000):
  if time.time()-started>600:raise RuntimeError('Debugger timeout; no automatic process termination requested')
  continue
 status=0x10002
 if e.code in (3,6):
  data=e.u.create if e.code==3 else e.u.dll;path=modulepath(data.file) if data.file else (exe if e.code==3 else '')
  modules.append({'path':path,'base':data.base or 0});D.SymLoadModuleExW(pi.process,data.file,path,None,data.base or 0,0,None,0)
  if data.file:K.CloseHandle(data.file)
 elif e.code==1:
  rec=e.u.exception.record;code=rec.code
  if code==0xc0000005:
   t=K.OpenThread(0x0048,False,e.tid);raw=c.create_string_buffer(1250);addr=(c.addressof(raw)+15)&~15;c.c_uint32.from_address(addr+48).value=0x10001f;stack=[]
   if K.GetThreadContext(t,addr):
    rip=U64.from_address(addr+248).value;rsp=U64.from_address(addr+152).value;rbp=U64.from_address(addr+160).value
    stack.append(symbolize(rip));sf=c.create_string_buffer(1024)
    for off,value in [(0,rip),(32,rbp),(48,rsp)]:a=ADDR.from_buffer(sf,off);a.offset=value;a.mode=3
    for i in range(40):
     if not D.StackWalk64(0x8664,pi.process,t,sf,addr,None,c.cast(D.SymFunctionTableAccess64,P),c.cast(D.SymGetModuleBase64,P),None):break
     pc=ADDR.from_buffer(sf).offset
     if not pc:break
     stack.append(symbolize(pc))
   else:stack=['GetThreadContext error '+str(c.get_last_error())]
   K.CloseHandle(t)
   f={'firstChance':e.u.exception.first,'code':hex(code),'address':hex(rec.address or 0),'thread':e.tid,'operation':int(rec.info[0]),'target':hex(rec.info[1]),'stack':stack};faults.append(f)
   (out/'Faults.json').write_text(json.dumps(faults,indent=2));(out/'Modules.json').write_text(json.dumps(modules,indent=2))
   h=K.CreateFileW(str(out/('Fault-'+str(len(faults))+'.dmp')),0x40000000,0,None,2,0,None)
   ok=D.MiniDumpWriteDump(pi.process,pi.pid,h,0x1020,None,None,None);K.CloseHandle(h)
   print(json.dumps({'fault':f,'dumpWritten':bool(ok)}),flush=True)
  if code!=0x80000003:status=0x80010001
 elif e.code==5:
  (out/'Exit.json').write_text(json.dumps({'pid':pi.pid,'exitCodeUnsigned':e.u.exit,'exitHex':hex(e.u.exit),'seconds':time.time()-started},indent=2));print('EXIT '+hex(e.u.exit),flush=True);K.ContinueDebugEvent(e.pid,e.tid,status);break
 K.ContinueDebugEvent(e.pid,e.tid,status)
