using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrentCharacterInfo
{
	public int currentCharacterCode;

	public CurrentCharacterInfo(int currentCharacterCode)
	{
		this.currentCharacterCode = currentCharacterCode;
	}
}
