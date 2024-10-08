﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MutationSelectHUD : MonoBehaviour
{
	public Image[] selectionIcons;

	public MutationInterface[] mutArray;

	public MutationSelect select;

	public MainMenu menu;

	public void selectMutation(int index)
	{
		if (index < mutArray.Length)
		{
			select.makeSelection(mutArray[index]);
		}
		else
		{
			select.makeSelection(null);
		}

		menu.resumeGame();
	}

	public void showMutationSelect(MutationSelect source)
	{
		Time.timeScale = 0;
		select = source;

		//save mutation array internally
		GameObject[] objArray = source.mutationPrefabs;
		mutArray = new MutationInterface[objArray.Length];

		//set icons
		for (int i = 0; i < objArray.Length; i++)
		{
			if (i < selectionIcons.Length)
			{
				MutationInterface mut = objArray[i].GetComponentInChildren<MutationInterface>();
				selectionIcons[i].sprite = mut.getIcon();
				mutArray[i] = mut;
			}
		}

		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(SceneDefs.MUTATION_SELECT_SCENE));
	}
}