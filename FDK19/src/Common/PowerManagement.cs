using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	/// <summary>
	/// システムとモニタの省電力制御を行う
	/// </summary>
	public static class PowerManagement
	{
		/// <summary>
		/// 本体/モニタの省電力モード移行を抑止する
		/// </summary>
		public static void DisableMonitorSuspend()
		{
			Win32Manager.SetThreadExecutionState( Win32Manager.ExecutionState.SystemRequired | Win32Manager.ExecutionState.DisplayRequired );
		}

		/// <summary>
		/// 本体/モニタの省電力モード移行抑制を解除する
		/// </summary>
		public static void EnableMonitorSuspend()
		{
			Win32Manager.SetThreadExecutionState( Win32Manager.ExecutionState.Continuous );		// スリープ抑止状態を解除
		}
	}
}
