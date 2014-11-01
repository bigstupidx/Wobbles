using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour
{
    const string credits =
    @"Wobbles 
by Play Nimbus

Creative Directors: 
Adam Roy
Nick Mudry
Laura Gagnon
Michael Flood
Rachel Roberie

Lead Programmer: 
Adam Roy

Lead Artist:
Laura Gagnon

Lead Level Designer: 
Michael Flood

Lead Quality Assurance: 
Michael Flood

Producer:
Nick Mudry

Initial Concept:
Jonathan Munoz

Asset Manager:
Adam Roy

Lead Environment Artist:
Rachel Roberie

Tileset Artist:
Rachel Roberie

UI Design:
Laura Gagnon

Character Design & Concept Artist:
Laura Gagnon

Cartoon Artist: 
Andrew Kim

Level Designer: 
Michael Flood
    Nick Mudry

Marketing & Public Relations:
Nick Mudry

Business Development:
Nick Mudry

Music Composition & Sound Design:
Jonathan Reed

QA Testers:
Monty Sharma            Walt Yarbrough
Bryce Jassmond           Sam Goodspeed
Stuart Ramgolam        Chris Leveille
Tif Entwistle              Kevin Hendricks
Jonathan Reed            Ajay Chadha
Jonathan Munoz           Caleb Garner
Ali Swei                      Meng Luo
Ryan Casey                  Walter Somol
    Breeze Grigas             Dillon Skiffington
Connie Hildreth            Andrew Kim
Brandon Cimino            Jamie Young
Kyle Teachen               Ryan Cropper


Special Thanks:
Dan Silvers - Boston FIG
Owen Macindoe - Boston FIG
Dave Bisceglia - The Tap Lab
Erik Asmussen - 82 Apps
Mike Cronin - Charter TV 3
Ziba Scott - Popcannibal
Luigi Gland - Popcannibal
Alex Schwartz - Owlchemy Labs
Michael Carriere - Zapdot Inc.
Benjamin Cavallari - ARC
Robert Ferrari - Bare Tree Media
Alex Schwartz - Owlchemy Labs
Kellian Adams - Green Door Labs
Caleb Garner - Part12 Studios
Elicia Basoli - Game PR Consultant
Jonathon Myers - Reactive Studios
Mark Sivak - Northeastern University
Walter Somol - Stomp Games
Ichiro Lambe - Dejobaan Games
Mark Sivak - Northeastern University
Marco Mereu - Digisocial
Dave Bisceglia - The Tap Lab
President Johnson - Becker College
Lt. Gov. Tim Murray

Mass DiGI:
Monty Sharma
Timothy Loew
Walt Yarbrough

Thanks For Playing!";

	// Use this for initialization
	void Start ()
    {
        tk2dTextMesh textMesh =  GetComponent<tk2dTextMesh>();
        textMesh.text = credits;
        textMesh.maxChars = credits.Length;
        textMesh.Commit();
	}
}
