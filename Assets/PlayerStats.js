#pragma strict

public var level : int = 0;
public var xp : int = 0;
public var strength : int = 0;

function addXP(enemyXP:int){
	xp += enemyXP;
}

function getXP(){
	return xp;
}