using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GM
{
    //Sense
    public enum Detected
    {
        Known = 0,      //Quando uno è sicuro abbassa la guardia
        None = 1,       //Proprio perchè non sente nulla resta un po' in allerta
        Unknown = 2,    //Non sa cos'è ma non lo ritiene una minaccia
        Player = 3      //Ovvio
    }

    //Noise
    //Indice minore, priorità minore
    public enum NoiseType
    {
        None,
        Hit,
        Step
    }

    //Condizione delle transizioni
    public enum Condition
    {
        Bool,
        Float,
        Detected
    }

    //Logica delle condizioni
    public enum Logic
    {
        NotEqual,
        Less,
        LessEqual,
        Equal,
        GreaterEqual,
        Greater,
    }

    //Patrol
    public enum EnterPointType
    {
        None,
        Next,
        Random,
        Closest,
    }

    public enum AnswerType
    {
        None,
        ID,
        Player,
        Return,
        Chase
    }

    public static class GameStaticManager
    {
        public static string GetStateIdle() => "Idle";
        public static string GetStatePatrol() => "Patrol";
        public static string GetStateSpin() => "Spin";
        public static string GetStateAlert() => "Alert";
        public static string GetStateCallHelp() => "CallHelp";
        public static string GetStateChat() => "Chat";
        public static string GetStateChase() => "Chase";
        public static string GetStateSearch() => "Search";
        public static string GetStateTake() => "Take";

        public const int _numberAnswers = 4; 
    }
}


