<div align="center">

# Aliens Are Coming To Davis!

</div>
<div align="center">
<img width="847" alt="Screenshot 2024-02-08 at 11 37 50 PM" src="https://github.com/MLivanos/BaysianBeliefNetworks/assets/59032623/d9b59373-c5a4-4692-9531-ae1ed8c48aa6">
<img width="847" alt="KaylaCelesteRoom" src="https://github.com/user-attachments/assets/27d66f25-a547-4401-be6c-b7b04972bfe4">
<img width="847" alt=Interview src="https://github.com/user-attachments/assets/4f7bc1c0-4386-4b6b-a18b-6f58e3094e52">

<br>

<p><a href="https://prodigiousmike.itch.io/bayesian-belief-network-simulation-alien-invasion"><img alt="Static Badge" src="https://img.shields.io/badge/PLAY-black?style=for-the-badge&logo=unity&logoColor=white&labelColor=black&color=black&link=mlivanos.github.io%2FSearchAlgorithmVisualizer%2F" style="width:225px;"/></a></p>

NOTE: The above button is for a minimal demo and not the final project

</div>

And we need your help to better understand what, if any, the threat is. In this interactive demonstration, we will build a Bayesian Belief Network to model the city of Davis, California with the potential for alien visitation. We will use the model to reason about when the aliens will come and what the warning signs will be to best prepare for these encounters.

This simulation is an exploration of Bayesian Belief Networks, an efficient model of joint probability distributions. Unlike problems in a textbook, this network is too large for exact with 10 nodes and 17 connections. Instead, we rely on sampling methods - rejection sampling and likelihood weighting - to help us estimate the probability of visitation with the information we know.

<div align="center">
<img width="847" alt="Graph" src="https://github.com/user-attachments/assets/1d4f8b57-c5c5-474e-b784-1e3327512349">
</div>

This picture is the network we are reasoning over. The aliens want a clear view of the ground, so they are less likely to visit during cloudy days. Similarly, they are less likely to visit on a busy day since people are around. While they have avoided detection from our technology, they often set off dogs who bark in their presence. This is not the only probability we can query, however. The dog barking is (weak) evidence of an alien invasion, but what if we can't see the dog? We might use information about the cat hiding as an indicator of the dog's status. Further, what if we lose access to our meteorological information? The shop closing early may be evidence of rain.

The above examples can be used to demonstrate the three different types of reasoning that can be done via Bayesian Belief Networks: prediction given a direct cause, diagnosis given evidence, and explaining conditions from alternative causes. For any node, calculate the probability P(N) by clicking the Q checkbox for that node (Q for query). To add something to evidence P(N|E), click the E node (E for evidence). The simulation supports any combination of nodes in the query and/or evidence (eg P(N1,N2,N3,..|E1,E2,E3,...), read as "the probability of N1, N2, N3, etc co-occurring given E1, E2, E3, etc"). To add the negation of a node, hold shift while clicking the checkbox. This simulation can also be useful for demonstrating conditional independence and Markov blankets. For any given node, find the Markov blanket and the probability of that node given the Markov blanket, then find the probability of the Markov blanket given anything else to convince yourself that a node is conditionally independent of the rest of the graph given the Markov blanket.
