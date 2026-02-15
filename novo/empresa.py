class Empresa:
    def __init__(self, nome, codigo, nif):
        self.nome = nome
        self.codigo = codigo
        self.nif = nif
        self.password_at = None
        self.password_at_encriptada = None
        self.niss = None
        self.password_ss = None
        self.password_ss_encriptada = None
        self.nome_do_responsavel = None
        self.telefone_do_responsavel = None
        self.email_do_responsavel = None
        self.codigo_certidao_permanente = None

    def __str__(self):
        return f"{self.nif} {self.nome}"

    def clone(self):
        import copy
        return copy.deepcopy(self)
