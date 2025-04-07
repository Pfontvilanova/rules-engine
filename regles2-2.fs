\ ***********************************************************************************
\ autor: Pere Font
\ data: 4/4/025
\ nom: sistema expert, modul presa aigua
\ ***********************************************************************************

\ RESERVOIR SIMULATION

variable level
0 level !
variable pump1
variable pump2
variable pump3
variable pump4
0 pump1 !
0 pump2 !
0 pump3 !
0 pump4 !

: level? level @ ;

\ CONDITIONS
: is-empty ( -- flag ) level? 25 < ;
: low-level level? dup 25 > swap 50 < and ;
: medium-level level? dup 50 > swap 75 < and ;
: high-level level? dup 75 > swap 95 < and ;
: full level? 95 > ;

\ ACTIONS
: .status cr ." pump1 = " pump1 @ . 
	  cr ." pump2 = " pump2 @ . 	
          cr ." pump3 = " pump3 @ . 
          cr ." pump4 = " pump4 @ .
;
: load  cr ." close all pumps " 0 pump1 ! 0 pump2 ! 0 pump3 ! 0 pump4 ! 
	  .status
;
: adjusting1 pump2 @ 0= if
          cr ." open pump1 " 1 pump1 !  
	  .status 
		else
	  cr ." close pump2 " 0 pump2 !
	 .status then
;
: adjusting2 pump3 @ 0= if
          cr ." open pump2 " 1 pump2 ! 
	  .status 
		else
	  cr ." close pump3 " 0 pump3 !
	  .status then
;
: adjusting3 pump4 @ 0= if
          cr ." open pump3 " 1 pump3 ! 
	  .status 
		else
	  cr ." close pump4 " 0 pump4 !
	  .status then
;
: toempty 
          cr ." abrir pump4 " 1 pump4 !
	  cr ." open all the pumps " 
	  .status
;
: new-level
	cr ." Enter de current level: "
	pad 10 accept >r 0. pad r> >number 2drop drop 
	level ! cr ." the new level is: " level? . cr
;



\ RULES SYSTEM


variable num-rule
0 num-rule !       
variable priority
0 priority !
variable rules-limit
20 rules-limit ! \ 20 rules by now

create conditions rules-limit cells allot 
conditions rules-limit cells erase
create actions rules-limit cells allot  
actions rules-limit cells erase
create mypad 32 allot 


 \ priority input
: position
    cr ." enter the priority of rule (less is more priority): " 
    pad 10 accept >r 0. pad r> >number 2drop drop 
    priority !
;

  : read pad 15 accept pad swap mypad place ;
  : compare mypad find  
    dup 0= if ." word does?t exist" cr exit then ;
  : >save swap drop num-rule @ cells + ! ; \ <name> >save

 \ condition input
  : condition   cr ." Enter the name of condition (defined previously): " 
     read  
     compare 
   \ drop num-rule @ cells conditions + ! 
     conditions >save
 ;   
  \ Action input
  : action cr ." Enter the nme of action (defined previously): " 
    read
    compare 
   \ drop num-rule @ cells actions + ! 
    actions >save
   ; 

 \ rules increment
: counterup priority @ 1 -  0 max num-rule ! ;
\ registering word
: rule-record ( -- ) position counterup condition action cr ." rule registered successfully n: " num-rule @ 1 + .
;


\ show the rules
: show-rules ( -- )
	priority @ num-rule @  or 0= if cr ." No rules " exit then
    cr ." rules are: " cr
    num-rule @ 1 +  0 do
        cr ." Rule " i 1 + . ." :"
        cr ."  Condition: " i cells conditions + @ >name count type
        cr ."  Action: " i cells actions + @ >name count type
        cr
    loop
;

\ inference engine
: executing ( -- )
    cr ." Executing the inference engine..." cr
     num-rule @  0= if ." No rules " exit then
    num-rule @  0 do
        i cells conditions + @ execute 
        if
        i cells actions + @ execute 
        else
           cr ." the rule condition " i 1+ . ." is not fulfilled." cr
        then
    loop
;

\ initial instructions
: configure ( -- ) cr
    cr ." before running the system you must enter the conditions and the actions"
    cr ." then you must register the rules and enter an initial level"
    cr ." finally you can show the rules or execute the rules"
    cr ." the last one is to execute the word SIMULATION"
		  cr
;
: input-max cr ." enter the max number of rules: " 
    pad 10 accept >r 0. pad r> >number 2drop drop 
    rules-limit !
;


\ Menu 
: mainmenu ( -- )
    begin
        cr ." Main Menu:"
	cr ." 0. enter max number of rules" \ only reservoir module
        cr ." 1. Instructions"
        cr ." 2. Register new rule"
        cr ." 3. Show rules"
        cr ." 4. To execute inference engine"
        cr ." 5. Exit"
        cr ." Choose one option (1-5): "
        key dup emit
        case
	    [char] 0 of input-max endof
            [char] 1 of configure endof
            [char] 2 of rule-record endof
            [char] 3 of show-rules endof
            [char] 4 of executing endof
            [char] 5 of exit endof
             cr ." invalid option, try again." cr
        endcase
    again
;

\ start the expert system menu
mainmenu

\ SIMULATION WORD
: SIMULATION begin new-level executing again ;


