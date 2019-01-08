namespace WestWorld1

module Entities =

    type EntityNames = Miner of int

    type Location =
        | Shack
        | GoldMine
        | Bank
        | Saloon

    [<AbstractClass>]
    type BaseGameEntity(id) =
    
        member x.Id = id

        abstract member Update: unit -> unit

    type Miner(id) =
        inherit BaseGameEntity(id)

        let maxNuggets = 3
        let thirstLevel = 5
        let tirednessThreshold = 5

        member val Thirst = 0 with get, set
        member val GoldCarried = 0 with get, set
        member val MoneyInBank = 0 with get, set
        member val Fatigue = 0 with get, set
        member val Location = Shack with get, set
        member val CurrentState = GoHomeAndSleepTilRested.Instance :> State with get, set

        member x.ComfortLevel = 5

        override x.Update() = 
            x.Thirst <- x.Thirst + 1
            x.CurrentState.Execute(x)

        member x.ChangeState newState =
            x.CurrentState.Exit(x)
            x.CurrentState <- newState
            x.CurrentState.Enter(x)

        member x.ChangeLocation(location) =
            x.Location <- location

        member x.PocketsFull() =
            x.GoldCarried >= maxNuggets
        member x.Gold() = x.GoldCarried
        member x.AddToGoldCarried(amount) =
            x.GoldCarried <- x.GoldCarried + amount

        member x.Wealth() = x.MoneyInBank
        member x.SetWealth(amount) = x.MoneyInBank <- amount
        member x.AddToWealth(amount) =
            x.MoneyInBank <- x.MoneyInBank + amount
            x.GoldCarried <- 0

        member x.Thirsty() = x.Thirst >= thirstLevel
        member x.BuyAndDrinkWhiskey() =
            x.Thirst <- 0
            x.MoneyInBank <- x.MoneyInBank - 2

        member x.Fatigued() = x.Fatigue >= tirednessThreshold
        member x.IncreaseFatigue() = x.Fatigue <- x.Fatigue + 1
        member x.DecreaseFatigue() = x.Fatigue <- x.Fatigue - 1

    and
        [<AbstractClass>]
        State() =
            abstract member Enter: Miner -> unit
            abstract member Execute: Miner -> unit
            abstract member Exit: Miner -> unit
    and
        EnterMineAndDigForNugget() =
            inherit State()

            static let instance = EnterMineAndDigForNugget()
            static member Instance = instance

            override x.Enter(miner) =
                // if the miner is not already located at the goldmine, he must
                // change location to the gold mine
                if not (miner.Location = GoldMine) then
                    printfn "%O: Walkin' to the gold mine" miner.Id
                    miner.ChangeLocation GoldMine

            override x.Execute(miner) =
                // the miner digs for gold until he is carrying in excess of MaxNuggets. 
                // If he gets thirsty during his digging he packs up work for a while and 
                // changes state to go to the saloon for a whiskey.
                miner.AddToGoldCarried 1
                miner.IncreaseFatigue()
                printfn "%O: Pickin' up a nugget" miner.Id

                // if enough gold mined, go and put it in the bank
                if miner.PocketsFull() then
                    miner.ChangeState VisitBankAndDepositGold.Instance
                
                if miner.Thirsty() then
                    miner.ChangeState QuenchThirst.Instance

            override x.Exit(miner) =
                printfn "%O: Ah'm leavin' the goldmine with mah pockets full o' sweet gold" miner.Id
    and 
        VisitBankAndDepositGold() =
            inherit State()

            static let instance = VisitBankAndDepositGold()
            static member Instance = instance

            override x.Enter(miner) =
                // on entry the miner makes sure he is located at the bank
                if not (miner.Location = Bank) then
                    printfn "%O: Goin' to the bank. Yes siree" miner.Id
                    miner.ChangeLocation Bank

            override x.Execute(miner) =
                // deposit the gold
                miner.AddToWealth miner.GoldCarried
                let wealth = miner.Wealth()
                printfn "%O: Depositing gold. Total savings now: %i" miner.Id wealth

                // wealthy enough to have a well earned rest?
                if miner.Wealth() >= miner.ComfortLevel then
                    printfn "%O: WooHoo! Rich enough for now. Back home to mah li'lle lady" miner.Id
                    miner.ChangeState GoHomeAndSleepTilRested.Instance
                else
                    miner.ChangeState EnterMineAndDigForNugget.Instance

            override x.Exit(miner) = printfn "%O: Leavin' the bank" miner.Id
                
    and
        GoHomeAndSleepTilRested() =
            inherit State()

            static let instance = GoHomeAndSleepTilRested()
            static member Instance = instance

            override x.Enter(miner) = 
                if not (miner.Location = Shack) then
                    printfn "%O: Walkin' home" miner.Id
                    miner.ChangeLocation Shack

            override x.Execute(miner) =
                // if miner is not fatigued start to dig for nuggets again.
                if miner.Fatigued() then
                    // sleep
                    miner.DecreaseFatigue()
                    printfn "%O: ZZZZ... " miner.Id
                else
                    printfn "%O: What a God darn fantastic nap! Time to find more gold" miner.Id
                    miner.ChangeState EnterMineAndDigForNugget.Instance

            override x.Exit(miner) = printfn "%O: Leaving the house" miner.Id
    and
        QuenchThirst() =
            inherit State()
            
            static let instance = QuenchThirst()
            static member Instance = instance

            override x.Enter(miner) =
                if not (miner.Location = Saloon) then
                    miner.ChangeLocation Saloon
                    printfn "%O: Boy, ah sure is thusty! Walking to the saloon" miner.Id

            override x.Execute(miner) =
                if miner.Thirsty() then
                    miner.BuyAndDrinkWhiskey()
                    printfn "%O: That's mighty fine sippin liquer" miner.Id
                    miner.ChangeState EnterMineAndDigForNugget.Instance
                else
                    printfn "ERROR!"

            override x.Exit(miner) = printfn "%O: Leaving the saloon, feelin' good" miner.Id
