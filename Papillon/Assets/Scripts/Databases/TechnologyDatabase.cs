﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TechnologyDatabase : MonoBehaviour {

    private static List<Technology> techList;

    public static void init(TextAsset reader) {
        techList = new List<Technology>();

        try {
            string[] lines;
            lines = reader.text.Split('\n');
            foreach (string line in lines)
            {
                // for database comment
                if (line[0] == '#')
                    continue;

                string[] words = line.Split(' ');

                // TECH DB : 'ID NAME DESCRIPTION POINT (REQUIRE1 REQUIRE2 ... )

                List<int> requirements = new List<int>();
                for (int i = 4; i < words.Length; i++)
                {
                    requirements.Add(int.Parse(words[i]));
                }

                techList.Add(new Technology(
                    int.Parse(words[0]),
                    words[1].Replace('_', ' '),
                    words[2].Replace('_', ' '),
                    int.Parse(words[3]),
                    requirements
                    ));
                
            }
        } catch (System.Exception e) {
            Debug.Log("Wrong File" + e);
        }
    }

    public static string findNameById(int id) {
        foreach (Technology tech in techList) {
            if (tech.getId() == id) {
                return tech.getName();
            }
        }

        return "notExists";
    }

    public static int findIdByName(string name) {
        foreach (Technology tech in techList) {
            if (tech.getName() == name) {
                return tech.getId();
            }
        }

        return 0;
    }

    public static Technology findTechnology(int id) {
        foreach(Technology tech in techList) {
            if(tech.getId() == id) {
                return tech;
            }
        }
        return new Technology(0, "notExists", "", 0, new List<int>());
    }

    public static List<Technology> load() {
        return techList;
    }
}
