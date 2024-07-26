import { Bid } from "@/types"

type State = {
    bids: Bid[]
}

type Actions = {
    setBids: (bids: Bid[]) => void  
    addBids: (bid: Bid) => void
}

export const useBidStore = create<State, Actions>((set) => ({
    bids: [],

    setBids: (bids: Bid[]) => {
        set(() => ({
            bids
        }))
     },

    addBids: (bid: Bid) => {
        set((state) => ({
            bids: !state.bods.find(x => x.id === bid.id) ? [bid, ...state.bids] : [...state.bids]
        }))
    }
}))